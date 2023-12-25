namespace {{Namespace}};

public static class {{EntityName}}Consts
{
{{#AllProperties}}
    {{#IsString}}
    public static int Max{{PropertyName}}Length { get; set; } = {{^MaxLengthOrPercise}}256{{/MaxLengthOrPercise}}{{#MaxLengthOrPercise}}{{.}}{{/MaxLengthOrPercise}};
        {{^IsLast}}

        {{/IsLast}}
    {{/IsString}}
{{/AllProperties}}
}