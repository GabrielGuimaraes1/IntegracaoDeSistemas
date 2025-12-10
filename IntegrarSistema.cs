namespace CronosERP.Service.Class.Sistema_a_Integrar
{
    public class IntegrarSistema_a_Integrar
    {
        public static void Executar(string cnnString)
        {
            var cronosDAO = new CronosDAODataContext(cnnString);

            var integracoes = GeneralMethodService.GetIntegration((int)BasePage.FornecedorSoftware.Sistema_a_Integrar, cronosDAO);

            if (integracoes != null)
            {
                var fornecedorIntegrado = integracoes.Item3;

                var listaAssociado = AssociadoBO.GetList(null, null, cronosDAO) as List<SP_Associado_ListResult>;

                var listaVeiculo = VeiculoBO.GetList(null, null, null, null, null, null, null, null, null, cronosDAO) as List<SP_Veiculo_ListResult>;

                var listaAgregado = AgregadoBO.GetList(null, null, null, null, cronosDAO);

                var listaEquipamento = RastreadorBO.GetList(null, null, null, null, cronosDAO) as List<SP_Rastreador_ListResult>;

                var listIntegrarAssociado = integracoes.Item1.Where(x => (x.IdTipoEntidade_IntegrFornec.Equals((int)BasePage.Entidade.Associado)
                  || x.IdTipoEntidade_IntegrFornec.Equals((int)BasePage.Entidade.CentralRastreamento))).ToList();

                var listaIntegrarVeiculo = integracoes.Item1.Where(x => (x.IdTipoEntidade_IntegrFornec.Equals((int)BasePage.Entidade.Veiculo)
                  || x.IdTipoEntidade_IntegrFornec.Equals((int)BasePage.Entidade.Agregado))).ToList();

                var listaIntegrarEquipamento = integracoes.Item2;

                if (listIntegrarAssociado != null && listIntegrarAssociado.Any())
                    Integrar(listIntegrarAssociado, listaVeiculo, listaAssociado, fornecedorIntegrado, cronosDAO);

                if (listaIntegrarVeiculo != null && listaIntegrarVeiculo.Any())
                    Integrar(listaIntegrarVeiculo, listaVeiculo, listaAssociado, fornecedorIntegrado, cronosDAO);
            }
        }

        private static void Integrar(List<SP_IntegracaoFornecedor_ListResult> listIntegrarEntidade, List<SP_Veiculo_ListResult> listaVeiculo, List<SP_Associado_ListResult> listaAssociado,
        SP_FornecedorIntegrado_InfResult fornecedorIntegrado, CronosDAODataContext cronosDAO)
        {
            var fornecedorIntegradoBO = new FornecedorIntegradoBO();
            DeviceGroup deviceListSistema = Sistema_a_IntegrarBO.GetRastreadores(fornecedorIntegrado);
            ReturnUser listUser = Usuario.GetUsuarios(fornecedorIntegrado);
            SP_Associado_ListResult associado = null;
            SP_Veiculo_ListResult veiculo = null;
            SP_RastreadorVeiculo_ListResult rasVeiculo = null;
            Mensagem mensagem = new Mensagem();
            mensagem.mensagem = "Falha ao integrar.";

            foreach (var integrarEntidade in listIntegrarEntidade)
            {
                try
                {
                    if (integrarEntidade.IdTipoEntidade_IntegrFornec != 1)
                        veiculo = listaVeiculo.Where(v => v.Id_Veiculo.Equals((int)integrarEntidade.IdEntidade_IntegrFornec)).FirstOrDefault();

                    if (veiculo == null)
                    {
                        associado = listaAssociado.FirstOrDefault(x => x.Id_Associado.Equals((int)integrarEntidade.IdEntidade_IntegrFornec));
                        if (associado.NotNull())
                            veiculo = listaVeiculo.FirstOrDefault(v => v.Id_Associado.Equals(associado.Id_Associado));
                    }
                    else if (integrarEntidade.IdTipoEntidade_IntegrFornec == 2)
                        associado = listaAssociado.FirstOrDefault(a => a.Id_Associado == veiculo.Id_Associado);

                    else if (integrarEntidade.IdTipoEntidade_IntegrFornec == 1)
                    {
                        associado = listaAssociado.FirstOrDefault(a => a.Id_Associado.Equals(veiculo.Id_Associado));
                    }

                    if (veiculo.NotNull())
                    {
                        rasVeiculo = RastreadorVeiculoBO.GetListTipada(veiculo.Id_Veiculo, null, null, fornecedorIntegrado.Id_FornecedorIntegrado, cronosDAO).FirstOrDefault();
                    }

                    if (associado != null || veiculo != null)
                    {

                        if (integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.Inclusao || integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.Pendente)
                        {
                            if (!String.IsNullOrEmpty(associado.Email_Associado))
                            {
                                User2 userData = Usuario.De_ParaUsuario(associado);
                                bool usuarioExiste = false;

                                foreach (User2 usuario in listUser.user)
                                {
                                    if (usuario.Email == associado.Email_Associado)
                                    {
                                        userData.Id = usuario.Id;
                                        usuarioExiste = true;
                                        break;
                                    }
                                }

                                if (usuarioExiste)
                                {
                                    ReturnUser putUser = Usuario.PutClient(fornecedorIntegrado, userData);
                                    if (putUser.NotNull())
                                    {
                                        if (putUser.Sucesso)
                                        {
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Usuário atualizado com sucesso.");
                                        }
                                        else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                    }
                                    else
                                        mensagem = Mensagem.mensagemSucesso(mensagem, "Falha ao integrar.");
                                }
                                else
                                {
                                    ReturnUser postUser = Usuario.PostUsuario(fornecedorIntegrado, userData);

                                    if (postUser.NotNull())
                                    {
                                        if (postUser.Sucesso)
                                            mensagem = Mensagem.mensagemSucesso(mensagem,"Usuário inserido com sucesso.");
                                        else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                    }
                                    else
                                        mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                }

                                DeviceGroup dispositivo = Dispositivo.De_ParaRastreador(rasVeiculo, veiculo, associado, fornecedorIntegrado);
                                ReturnDispositivo retorno = null;
                                Device ret = new Device();
                                bool deviceExiste = false;

                                for (int i = 0; i < deviceListSistema.devices.Count(); i++)
                                {
                                    Device devs = deviceListSistema.devices[i];

                                    for (int l = 0; l < deviceListSistema.devices[i].items.Count(); l++)
                                    {
                                        Item its = deviceListSistema.devices[i].items[l];

                                        if (deviceListSistema.devices[i].items[l].deviceData.imei == dispositivo.devices[0].items[0].deviceData.imei)
                                        {
                                            dispositivo.devices[0].items[0].deviceData.id = deviceListSistema.devices[i].items[l].deviceData.id;
                                            break;
                                        }
                                    }
                                }

                                if (dispositivo.NotNull())
                                {
                                    if (dispositivo.devices[0].items[0].deviceData.id.NotNull())
                                    {
                                        deviceExiste = true;
                                    }

                                    else
                                    {
                                        for (int i = 0; i < deviceListSistema.devices.Count(); i++)
                                        {
                                            Device devices = deviceListSistema.devices[i];

                                            for (int l = 0; l < devices.items.Count(); l++)
                                            {
                                                Item items = deviceListSistema.devices[i].items[l];

                                                if (items.deviceData.NotNull())
                                                {
                                                    if (items.deviceData.plateNumber.NotNull())
                                                    {
                                                        if (items.deviceData.plateNumber.Replace("-", "").Replace(" ", "") == veiculo.Placa_Veiculo.Replace(" ", "").Replace("-", ""))
                                                        {
                                                            dispositivo.devices[0].items[0].deviceData.id = deviceListSistema.devices[i].items[l].deviceData.id;
                                                            deviceExiste = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            if (deviceExiste)
                                                break;
                                        }
                                    }
                                }


                                if (!deviceExiste)
                                {

                                    retorno = Sistema_a_IntegrarBO.CriarRastreador(fornecedorIntegrado, rasVeiculo, associado, veiculo);

                                    if (retorno.Sucesso)
                                        mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo inserido.");
                                    else
                                    {
                                        retorno = Sistema_a_IntegrarBO.AtualizarRastreador(fornecedorIntegrado, rasVeiculo, associado, veiculo, dispositivo);

                                        if (retorno.Sucesso)
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo atualizado.");
                                        else
                                        {
                                            int id_rastreador = int.Parse(integrarEntidade.Id_Servico.ToString());
                                            SP_Rastreador_InfResult rastreador = RastreadorBO.GetThisCadMassa(id_rastreador, null, cronosDAO);
                                            dispositivo = Dispositivo.De_ParaRastreadorVincDesvinc(rastreador, veiculo, associado, fornecedorIntegrado);
                                            if (dispositivo != null)
                                            {
                                                dispositivo.devices[0].items[0].deviceData.id = integrarEntidade.IdEntidadeRetorno_IntegrFornec;
                                                //dispositivo.devices[0].items[0].deviceData.pivot.userId = integrarEntidade.IdClienteRetorno_IntegrFornec;
                                                fornecedorIntegrado.Dados_FornecedorIntegrado = integrarEntidade.IdClienteRetorno_IntegrFornec.ToString();
                                            }
                                            retorno = Sistema_a_IntegrarBO.Vincular_Cliente(fornecedorIntegrado, rastreador, veiculo, dispositivo, associado);

                                            if (retorno.Sucesso)
                                                mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo vinculado.");
                                            else
                                                mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                        }
                                    }
                                }
                                else
                                {
                                    retorno = Sistema_a_IntegrarBO.AtualizarRastreador(fornecedorIntegrado, rasVeiculo, associado, veiculo, dispositivo);

                                    if (retorno != null)
                                    {
                                        if (retorno.Sucesso)
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo atualizado.");
                                        else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                    }
                                }

                            }
                            else
                                mensagem.mensagem += "Cliente não possui email.";
                        }

                        if (integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.ConsultarIdRetorno)
                        {
                            if (!String.IsNullOrEmpty(associado.Email_Associado))
                            {
                                DeviceGroup dispositivo = Dispositivo.De_ParaRastreador(rasVeiculo, veiculo, associado, fornecedorIntegrado);
                                Item dispProcurado = Sistema_a_IntegrarBO.GetRastreador(fornecedorIntegrado, dispositivo.devices.FirstOrDefault().items[0], associado);

                                if (dispProcurado.NotNull())
                                    if (dispProcurado.id.NotNull())
                                        mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo encontrado.");
                                    else
                                        mensagem = Mensagem.mensagemErro(mensagem, "Falha ao buscar dispositivo.");
                            }
                            else
                                mensagem = Mensagem.mensagemErro(mensagem, "Cliente não possui email.");
                        }

                        if (integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.Ativar||
                            integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.Inativar)
                        {
                            
                            if (!String.IsNullOrEmpty(associado.Email_Associado))
                            {
                                if (integrarEntidade.IdTipoEntidade_IntegrFornec == 1)
                                {
                                    User2 userData = Usuario.De_ParaUsuario(associado);
                                    bool usuarioExiste = false;
                                    ReturnUser putUser = new ReturnUser();

                                    foreach (User2 usuario in listUser.user)
                                    {
                                        if (usuario.Email == associado.Email_Associado)
                                        {
                                            userData.Id = usuario.Id;
                                            usuarioExiste = true;
                                        }
                                    }

                                    if (usuarioExiste)
                                    {
                                        if (integrarEntidade.Status_IntegrFornec == 8)
                                        {
                                            userData.Active = false;
                                        }
                                        else if (integrarEntidade.Status_IntegrFornec == 7)
                                        {
                                            userData.Active = true;
                                        }

                                        putUser = Usuario.PutClientStatus(fornecedorIntegrado, userData);

                                        if (putUser.NotNull())
                                        {
                                            if (putUser.Sucesso)
                                            {
                                                mensagem = Mensagem.mensagemSucesso(mensagem, "Usuário atualizado com sucesso.");
                                            }
                                            else
                                                mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                        }
                                        else
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Falha ao integrar.");
                                    }
                                    else
                                    {
                                        ReturnUser postUser = Usuario.PostUsuario(fornecedorIntegrado, userData);

                                        if (postUser.NotNull() && postUser.Sucesso)
                                        {
                                            putUser = Usuario.PutClientStatus(fornecedorIntegrado, userData);
                                            if (putUser.NotNull() && putUser.Sucesso)
                                            {
                                                mensagem = Mensagem.mensagemSucesso(mensagem, "Usuário ativado.");
                                            }
                                            else
                                                mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                        }
                                        else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                    }
                                }

                                if (integrarEntidade.IdTipoEntidade_IntegrFornec == 2)
                                { 
                                    DeviceGroup dispositivo = Dispositivo.De_ParaRastreador(rasVeiculo, veiculo, associado, fornecedorIntegrado);
                                    ReturnDispositivo retorno = null;

                                    if (integrarEntidade.Status_IntegrFornec == 8)
                                        dispositivo.devices[0].items[0].active = "0";
                                    else if (integrarEntidade.Status_IntegrFornec == 7)
                                        dispositivo.devices[0].items[0].active = "1";

                                    retorno = Dispositivo.PutDeviceStatus(fornecedorIntegrado, rasVeiculo, veiculo, dispositivo);

                                    if (retorno.Sucesso)
                                    {
                                        mensagem = Mensagem.mensagemSucesso(mensagem, "Veículo ativado.");
                                    }
                                    else
                                        mensagem.mensagem = "Falha ao integrar.";
                                }
                            }
                        }

                        if (integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.Transferencia
                            || integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.TransfCentralRastr)
                        {
                            if (!String.IsNullOrEmpty(associado.Email_Associado))
                            {
                                User2 userData = Usuario.De_ParaUsuario(associado);
                                bool usuarioExiste = false;

                                foreach (User2 usuario in listUser.user)
                                {
                                    if (usuario.Email.ToLower() == associado.Email_Associado.ToLower())
                                    {
                                        userData.Id = usuario.Id;
                                        usuarioExiste = true;
                                        break;
                                    }
                                }

                                if (usuarioExiste)
                                {
                                    ReturnUser putUser = Usuario.PutClient(fornecedorIntegrado, userData);
                                    if (putUser.NotNull())
                                    {
                                        if (putUser.Sucesso)
                                        {
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Usuário atualizado com sucesso.");
                                        }
                                        else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                    }
                                    else
                                        mensagem = Mensagem.mensagemSucesso(mensagem, "Falha ao integrar.");
                                }
                                else
                                {
                                    ReturnUser postUser = Usuario.PostUsuario(fornecedorIntegrado, userData);

                                    if (postUser.NotNull())
                                    {
                                        if (postUser.Sucesso)
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Usuário inserido com sucesso.");
                                        else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                    }
                                    else
                                        mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                }

                                DeviceGroup dispositivo = Dispositivo.De_ParaRastreador(rasVeiculo, veiculo, associado, fornecedorIntegrado);
                                ReturnDispositivo retorno = null;
                                Device ret = new Device();
                                bool deviceExiste = false;

                                for (int i = 0; i < deviceListSistema.devices.Count(); i++)
                                {
                                    Device devs = deviceListSistema.devices[i];

                                    for (int l = 0; l < deviceListSistema.devices[i].items.Count(); l++)
                                    {
                                        Item its = deviceListSistema.devices[i].items[l];

                                        if (deviceListSistema.devices[i].items[l].deviceData.imei == dispositivo.devices[0].items[0].deviceData.imei)
                                        {
                                            dispositivo.devices[0].items[0].deviceData.id = deviceListSistema.devices[i].items[l].deviceData.id;
                                            deviceExiste = true;
                                            break;
                                        }
                                    }
                                    if (deviceExiste)
                                        break;
                                }

                                if (!deviceExiste)
                                {

                                    retorno = Sistema_a_IntegrarBO.CriarRastreador(fornecedorIntegrado, rasVeiculo, associado, veiculo);

                                    if (retorno.Sucesso)
                                        mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo inserido.");
                                    else
                                    {
                                        retorno = Sistema_a_IntegrarBO.AtualizarRastreador(fornecedorIntegrado, rasVeiculo, associado, veiculo, dispositivo);

                                        if (retorno.Sucesso)
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo atualizado.");
                                        else
                                        {
                                            if (retorno.Mensagem.Contains("Dispositivo não encontrado"))
                                            {
                                                mensagem = Mensagem.mensagemErro(mensagem, retorno.Mensagem);
                                            }
                                            else
                                                mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                        }
                                    }
                                }
                                else
                                {
                                    retorno = Sistema_a_IntegrarBO.AtualizarRastreador(fornecedorIntegrado, rasVeiculo, associado, veiculo, dispositivo);

                                    if (retorno != null)
                                    {
                                        if (retorno.Sucesso)
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo atualizado.");
                                        else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                    }
                                    else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                }

                            }
                            else
                                mensagem.mensagem += "Cliente não possui email.";
                        }

                        if (integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.DesvinculaRastreador)
                        {
                            int id_rastreador = int.Parse(integrarEntidade.Id_Servico.ToString());
                            SP_Rastreador_InfResult rastreador = RastreadorBO.GetThisCadMassa(id_rastreador, null, cronosDAO);
                            
                            bool deviceExiste = false;
                            DeviceGroup dispositivo = Dispositivo.De_ParaRastreadorVincDesvinc(rastreador, veiculo, associado, fornecedorIntegrado);
                            ReturnDispositivo retorno = null;
                            DeviceGroup disp = new DeviceGroup();
                            Item item = Sistema_a_IntegrarBO.GetRastreador(fornecedorIntegrado, dispositivo.devices[0].items[0], associado);


                            for (int i = 0; i < deviceListSistema.devices.Count(); i++)
                            {
                                Device devs = deviceListSistema.devices[i];

                                for (int l = 0; l < deviceListSistema.devices[i].items.Count(); l++)
                                {
                                    Item its = deviceListSistema.devices[i].items[l];

                                    if (deviceListSistema.devices[i].items[l].deviceData.imei == dispositivo.devices[0].items[0].deviceData.imei)
                                    {
                                        dispositivo.devices[0].items[0].deviceData.id = deviceListSistema.devices[i].items[l].deviceData.id;
                                        deviceExiste = true;
                                        break;
                                    }
                                }
                                if (dispositivo.devices[0].items[0].deviceData.id != null)
                                    break;
                            }

                            if (deviceExiste && dispositivo.NotNull())
                             {
                                retorno = Sistema_a_IntegrarBO.Desvincular_Cliente(fornecedorIntegrado, rastreador, veiculo, dispositivo);

                                if (retorno.Sucesso)
                                {
                                    integrarEntidade.IdEntidadeRetorno_IntegrFornec = int.Parse(dispositivo.devices[0].items[0].deviceData.id.ToString());
                                    if (item != null && item.deviceData.pivot.userId != null)
                                        integrarEntidade.IdClienteRetorno_IntegrFornec = int.Parse(item.deviceData.pivot.userId.ToString());
                                    mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo desvinculado.");
                                }
                                else
                                    mensagem.mensagem = "Falha ao desvincular.";
                            }
                            else
                            {
                                mensagem = Mensagem.mensagemErro(mensagem, "Falha ao desvincular.");
                                            
                            }
                        }

                        if (integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.VinculaRastreador)
                        {
                            int id_rastreador = int.Parse(integrarEntidade.Id_Servico.ToString());
                            SP_Rastreador_InfResult rastreador = RastreadorBO.GetThis(id_rastreador, null);

                            bool deviceExiste = false;
                            DeviceGroup dispositivo = Dispositivo.De_ParaRastreador(rasVeiculo, veiculo, associado, fornecedorIntegrado);
                            ReturnDispositivo retorno = null;
                            DeviceGroup disp = new DeviceGroup();

                            for (int i = 0; i < deviceListSistema.devices.Count(); i++)
                            {
                                Device devs = deviceListSistema.devices[i];

                                for (int l = 0; l < deviceListSistema.devices[i].items.Count(); l++)
                                {
                                    Item its = deviceListSistema.devices[i].items[l];

                                    if (deviceListSistema.devices[i].items[l].deviceData.imei == dispositivo.devices[0].items[0].deviceData.imei)
                                    {
                                        dispositivo.devices[0].items[0].deviceData.id = deviceListSistema.devices[i].items[l].deviceData.id;
                                        deviceExiste = true;
                                        break;
                                    }
                                }
                                if (dispositivo.devices[0].items[0].deviceData.id != null)
                                    break;
                            }

                            if (deviceExiste && dispositivo.NotNull())
                            {
                                if (integrarEntidade.IdClienteRetorno_IntegrFornec.HasValue)
                                    dispositivo.devices[0].items[0].deviceData.pivot.userId = int.Parse(integrarEntidade.IdClienteRetorno_IntegrFornec.ToString());
                                retorno = Sistema_a_IntegrarBO.Vincular_Cliente(fornecedorIntegrado, rastreador, veiculo, dispositivo, associado);

                                if (retorno.Sucesso)
                                {
                                    mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo desvinculado.");
                                }
                                else
                                    mensagem.mensagem = "Falha ao vincular usuario a dispositivo.";
                            }
                            else
                            {
                                mensagem = Mensagem.mensagemErro(mensagem, "Falha ao vincular usuário a dispositivo.");

                            }
                        }

                        if (integrarEntidade.Status_IntegrFornec == (int)BasePage.StatusIntegrFornec.AtualizarCliente)
                        {
                            if (!String.IsNullOrEmpty(associado.Email_Associado))
                            {
                                User2 userData = Usuario.De_ParaUsuario(associado);
                                bool usuarioExiste = false;

                                foreach (User2 usuario in listUser.user)
                                {
                                    if (usuario.Email.Trim().ToLower() == associado.Email_Associado.Trim().ToLower())
                                    {
                                        userData.Id = usuario.Id;
                                        usuarioExiste = true;
                                        break;
                                    }
                                }

                                if (usuarioExiste)
                                {
                                    ReturnUser putUser = Usuario.PutClient(fornecedorIntegrado, userData);
                                    if (putUser.NotNull())
                                    {
                                        if (putUser.Sucesso)
                                        {
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Usuário atualizado com sucesso.");
                                        }
                                        else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                    }
                                    else
                                        mensagem = Mensagem.mensagemSucesso(mensagem, "Falha ao integrar.");
                                }
                                else
                                {
                                    ReturnUser postUser = Usuario.PostUsuario(fornecedorIntegrado, userData);

                                    if (postUser.NotNull())
                                    {
                                        if (postUser.Sucesso)
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Usuário inserido com sucesso.");
                                        else
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                    }
                                    else
                                        mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar.");
                                }

                                bool deviceExiste = false;
                                DeviceGroup dispositivo = Dispositivo.De_ParaRastreador(rasVeiculo, veiculo, associado, fornecedorIntegrado);
                                ReturnDispositivo retorno = null;
                                DeviceGroup disp = new DeviceGroup();

                                if (dispositivo.NotNull())
                                {
                                    if (dispositivo.devices[0].items[0].deviceData.id.NotNull())
                                    {
                                        deviceExiste = true;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < deviceListSistema.devices.Count(); i++)
                                        {
                                            Device devs = deviceListSistema.devices[i];

                                            for (int l = 0; l < deviceListSistema.devices[i].items.Count(); l++)
                                            {
                                                Item its = deviceListSistema.devices[i].items[l];

                                                if (deviceListSistema.devices[i].items[l].deviceData.imei == dispositivo.devices[0].items[0].deviceData.imei)
                                                {
                                                    dispositivo.devices[0].items[0].deviceData.id = deviceListSistema.devices[i].items[l].deviceData.id;
                                                    break;
                                                }
                                            }
                                        }

                                        if (dispositivo.NotNull())
                                        {
                                            if (dispositivo.devices[0].items[0].deviceData.id.NotNull())
                                            {
                                                deviceExiste = true;
                                            }
                                        }

                                        else
                                        {
                                            for (int i = 0; i < deviceListSistema.devices.Count(); i++)
                                            {
                                                Device devices = deviceListSistema.devices[i];

                                                for (int l = 0; l < devices.items.Count(); l++)
                                                {
                                                    Item items = deviceListSistema.devices[i].items[l];

                                                    if (items.deviceData.plateNumber.Replace("-", "").Replace(" ", "") == veiculo.Placa_Veiculo.Replace(" ", "").Replace("-", ""))
                                                    {
                                                        dispositivo.devices[0].items[0].deviceData.id = deviceListSistema.devices[i].items[l].deviceData.id;
                                                        deviceExiste = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (deviceExiste && dispositivo.NotNull())
                                {
                                    retorno = Sistema_a_IntegrarBO.AtualizarRastreador(fornecedorIntegrado, rasVeiculo, associado, veiculo, dispositivo);
                                    if (retorno.Sucesso)
                                    {
                                        mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo atualizado.");
                                    }
                                    else
                                        mensagem.mensagem = "Falha ao integrar.";
                                }
                                else
                                {
                                    retorno = Sistema_a_IntegrarBO.CriarRastreador(fornecedorIntegrado, rasVeiculo, associado, veiculo);

                                    if (retorno != null)
                                    {
                                        if (retorno.Sucesso)
                                            mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo integrado.");
                                        else
                                        {
                                            mensagem = Mensagem.mensagemErro(mensagem, "Falha ao integrar." + retorno.Mensagem);
                                            if (retorno.Mensagem.Length > 0)
                                            {
                                                if (retorno.Mensagem.Contains("Tentativa de inserção de chave duplicada."))
                                                {
                                                    retorno = Sistema_a_IntegrarBO.AtualizarRastreador(fornecedorIntegrado, rasVeiculo, associado, veiculo, dispositivo);

                                                    if (retorno.Sucesso)
                                                        mensagem = Mensagem.mensagemSucesso(mensagem, "Dispositivo integrado.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                                mensagem.mensagem = "Cliente não possui email.";
                        }


                        integrarEntidade.DataIntegracao_IntegrFornec = DateTime.Now;
                        integrarEntidade.Status_IntegrFornec = (int)StatusIntegrFornec.Concluido;
                        integrarEntidade.DescricaoErro_IntegrFornec = mensagem.mensagem;

                        FornecedorIntegradoBO.UpdateStatic(integrarEntidade, cronosDAO);
                    }
                    else
                    {
                        integrarEntidade.DescricaoErro_IntegrFornec = "Erro ao integrar - Cliente Cronos não encontrado.";
                        integrarEntidade.DataIntegracao_IntegrFornec = DateTime.Now;
                        integrarEntidade.Status_IntegrFornec = (int)BasePageIntegration.StatusIntegrFornec.Concluido;

                        FornecedorIntegradoBO.UpdateStatic(integrarEntidade, cronosDAO);

                    }
                }
                catch (Exception ex)
                {
                    integrarEntidade.DescricaoErro_IntegrFornec = "Erro ao integrar - Objeto nulo deve conter um Valor.";
                    integrarEntidade.DataIntegracao_IntegrFornec = DateTime.Now;
                    integrarEntidade.Status_IntegrFornec = (int)BasePageIntegration.StatusIntegrFornec.Concluido;

                    FornecedorIntegradoBO.UpdateStatic(integrarEntidade, cronosDAO);
                }
            }
        }


    }

    internal class Mensagem
    {
        public bool error { get; set; }
        public int code { get; set; }
        public string type { get; set; }
        public string mensagem { get; set; }


        internal static Mensagem mensagemErro(Mensagem mensagem, string texto)
        {
            mensagem.error = true;
            mensagem.code = 0;
            mensagem.type = "Catch";
            mensagem.mensagem = texto;

            return mensagem;
        }

        internal static Mensagem mensagemSucesso(Mensagem mensagem, string texto = null)
        {
            mensagem.error = false;
            mensagem.code = 1;
            mensagem.type = "Sucess";
            mensagem.mensagem = string.IsNullOrEmpty(texto) ? "Ok." : texto;


            return mensagem;
        }

    }
}
