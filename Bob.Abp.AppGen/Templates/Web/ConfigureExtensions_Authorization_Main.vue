            //Pages/{{RelativePath}}/{{EntityName}}.Authorization
            options.Conventions.AuthorizePage("/{{LastEntityGroup.RelativePath}}/{{EntityName}}/Index", {{ModuleName}}Permissions.{{EntityName}}.Default);
            options.Conventions.AuthorizePage("/{{LastEntityGroup.RelativePath}}/{{EntityName}}/CreateModal", {{ModuleName}}Permissions.{{EntityName}}.Create);
            options.Conventions.AuthorizePage("/{{LastEntityGroup.RelativePath}}/{{EntityName}}/EditModal", {{ModuleName}}Permissions.{{EntityName}}.Update);
