
    private static void Configure{{EntityName}}(ModelBuilder builder)
    {
        builder.Entity<{{EntityName}}>(b =>
        {
            //Configure table & schema name
            b.ToTable({{ModuleName}}DbProperties.DbTablePrefix + "{{ToPlural EntityName}}", {{ModuleName}}DbProperties.DbSchema);
            b.ConfigureByConvention();
    {{#AllProperties}}
        {{#IsString}}
            b.Property(q => q.{{PropertyName}}){{#Required}}.IsRequired(){{/Required}}.HasMaxLength({{EntityName}}Consts.Max{{PropertyName}}Length);
        {{/IsString}}
    {{/AllProperties}}

            //Relations
            //b.HasMany<{{EntityName}}>().WithOne().HasForeignKey(p => p.ParentId).OnDelete(DeleteBehavior.ClientSetNull);

            //Indexes
            //b.HasIndex(q => q.Code);
        });
    }
