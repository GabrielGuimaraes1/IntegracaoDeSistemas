public static DeviceGroup De_ParaRastreador(SP_RastreadorVeiculo_ListResult rastreador, SP_Veiculo_ListResult veiculo, SP_Associado_ListResult associado, SP_FornecedorIntegrado_InfResult fornecedorIntegrado)
        {
            if (rastreador != null)
            {
                try
                {
                    if (veiculo.SituacaoVeiculo == "Inativo" || veiculo.SituacaoVeiculo == "0")
                        veiculo.SituacaoVeiculo = "0";
                    else
                        veiculo.SituacaoVeiculo = "1";

                    DeviceGroup device = new DeviceGroup
                    {
                        devices = new List<Device>
                        {
                            new Device
                            {
                                id = null,
                                title = "",
                                items = new List<Item>
                                {
                                    new Item
                                    {
                                        name = associado.Nome_Associado,
                                        driverData = null,
                                        deviceData = new DeviceData
                                        {
                                            plateNumber = veiculo.Placa_Veiculo,
                                            vin = veiculo.Nome_ModeloVeiculo,
                                            imei = rastreador.IMEI_EquipamentoRastreio,
                                            simNumber = rastreador.Numero_Chip,
                                            registrationNumber = rastreador.NumeroSerie_EquipamentoRastreio,
                                            active = long.Parse(veiculo.SituacaoVeiculo.ToString()),
                                            fuelMeasurementId = 1,
                                            iconId = 0,
                                            tailLength = (decimal)0.00,
                                            minFuelFillings = (decimal)0.00,
                                            minFuelThefts = (decimal)0.00,
                                            minMovingSpeed = (decimal)0.0000,
                                            msisdn = null,
                                            deviceModel = rastreador.Descricao_ModeloRastreador,
                                            additionalNotes = associado.Email_Associado + ";" + associado.EmailOpc_Associado,
                                            users = new List<User>
                                            {
                                                new User
                                                {
                                                    email = associado.Email_Associado
                                                }
                                            },
                                            driver = null
                                        }
                                    }
                                }
                            }
                        }
                    };

                    return device;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    Item deviceCompleto = null;
                    Device devCompleto = null;
                    DeviceGroup device = new DeviceGroup
                    {
                        devices = new List<Device>
                        {
                            new Device
                            {
                                id = null,
                                title = "",
                                items = new List<Item>
                                {
                                    new Item
                                    {
                                        name = veiculo.Nome_ModeloVeiculo,
                                        driverData = null,
                                        deviceData = new DeviceData
                                        {
                                            plateNumber = veiculo.Placa_Veiculo,
                                            vin = veiculo.Nome_ModeloVeiculo,
                                            imei = "",
                                            simNumber = "",
                                            registrationNumber = "",
                                            active = long.Parse(veiculo.SituacaoVeiculo.ToString()),
                                            fuelMeasurementId = 1,
                                            iconId = 0,
                                            tailLength = (decimal)0.00,
                                            minFuelFillings = (decimal)0.00,
                                            minFuelThefts = (decimal)0.00,
                                            minMovingSpeed = (decimal)0.0000,
                                            msisdn = null,
                                            deviceModel = "",
                                            additionalNotes = associado.Email_Associado,
                                            users = new List<User>
                                            {
                                                new User
                                                {
                                                    email = associado.Email_Associado
                                                }
                                            },
                                            driver = null
                                        }
                                    }
                                }
                            }
                        }
                    };

                    deviceCompleto = GetDispositivo(fornecedorIntegrado, device.devices[0].items[0], associado);

                    if (deviceCompleto != null)
                    {
                        device.devices[0].items[0] = deviceCompleto;
                    }
                    else
                    {
                        devCompleto = GetDispositivos_Alterados(fornecedorIntegrado);
                        foreach (var item in devCompleto.items)
                        {
                            if (item.deviceData.plateNumber.Replace(" ", "").Replace("-", "") == device.devices[0].items[0].deviceData.plateNumber.Replace(" ", "").Replace("-", ""))
                            {
                                device.devices[0].items[0] = item;
                                break;
                            }
                        }

                    }
                    return device;

                }
                catch (Exception e)
                {
                    return null;
                }
            }

        }
