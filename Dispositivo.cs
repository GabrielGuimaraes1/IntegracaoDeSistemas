public partial class DeviceGroup
    {
        [JsonProperty("devices", NullValueHandling = NullValueHandling.Ignore)]
        public List<Device> devices { get; set; }
    }

    public partial class Device
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? id { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string title { get; set; }

        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public List<Item> items { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? id { get; set; }

        [JsonProperty("alarm", NullValueHandling = NullValueHandling.Ignore)]
        public long? alarm { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public Icon icon { get; set; }

        [JsonProperty("driver_data", NullValueHandling = NullValueHandling.Ignore)]
        public DriverData driverData { get; set; }

        [JsonProperty("device_data", NullValueHandling = NullValueHandling.Ignore)]
        public DeviceData deviceData { get; set; }

        [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
        public string active { get; set; }
    }

    public partial class DeviceData
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? id { get; set; }

        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? userId { get; set; }

        [JsonProperty("current_driver_id")]
        public object currentDriverId { get; set; }

        [JsonProperty("timezone_id")]
        public object timezoneId { get; set; }

        [JsonProperty("traccar_device_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? traccarDeviceId { get; set; }

        [JsonProperty("icon_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? iconId { get; set; }

        [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
        public long? active { get; set; }

        [JsonProperty("kind", NullValueHandling = NullValueHandling.Ignore)]
        public long? kind { get; set; }

        [JsonProperty("deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? deleted { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }

        [JsonProperty("imei", NullValueHandling = NullValueHandling.Ignore)]
        public string imei { get; set; }

        [JsonProperty("sim_number", NullValueHandling = NullValueHandling.Ignore)]
        public string simNumber { get; set; }

        [JsonProperty("msisdn")]
        public object msisdn { get; set; }

        [JsonProperty("plate_number", NullValueHandling = NullValueHandling.Ignore)]
        public string plateNumber { get; set; }

        [JsonProperty("vin", NullValueHandling = NullValueHandling.Ignore)]
        public string vin { get; set; }

        [JsonProperty("registration_number", NullValueHandling = NullValueHandling.Ignore)]
        public string registrationNumber { get; set; }

        [JsonProperty("object_owner", NullValueHandling = NullValueHandling.Ignore)]
        public string objectOwner { get; set; }

        [JsonProperty("additional_notes", NullValueHandling = NullValueHandling.Ignore)]
        public string additionalNotes { get; set; }

        [JsonProperty("fuel_measurement_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? fuelMeasurementId { get; set; }

        [JsonProperty("tail_length", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? tailLength { get; set; }

        [JsonProperty("min_fuel_fillings", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? minFuelFillings { get; set; }

        [JsonProperty("min_fuel_thefts", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? minFuelThefts { get; set; }

        [JsonProperty("min_moving_speed", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? minMovingSpeed { get; set; }

        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string comment { get; set; }

        [JsonProperty("expiration_date")]
        public object expirationDate { get; set; }

        [JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? createdAt { get; set; }

        [JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? updatedAt { get; set; }

        [JsonProperty("forward")]
        public object forward { get; set; }

        [JsonProperty("device_type_id")]
        public object deviceTypeId { get; set; }

        [JsonProperty("app_tracker_login", NullValueHandling = NullValueHandling.Ignore)]
        public long? appTrackerLogin { get; set; }

        [JsonProperty("pivot", NullValueHandling = NullValueHandling.Ignore)]
        public Pivot pivot { get; set; }

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public Icon icon { get; set; }

        [JsonProperty("driver")]
        public object driver { get; set; }

        [JsonProperty("group_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? groupId { get; set; }

        [JsonProperty("device-model", NullValueHandling = NullValueHandling.Ignore)]
        public string deviceModel { get; set; }

        [JsonProperty("users", NullValueHandling = NullValueHandling.Ignore)]
        public List<User> users { get; set; }
    }
