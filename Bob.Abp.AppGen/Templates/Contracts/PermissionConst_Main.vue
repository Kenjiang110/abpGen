    public static class {{EntityName}}
    {
        public const string Default = GroupName + ".{{EntityName}}";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
{{#IsHierarcy}}
        public const string Move = Default + ".Move";
{{/IsHierarcy}}
    }

