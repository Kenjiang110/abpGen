/*!
 * jQuery Validation Plugin Validate Group Extension v0.1
 *
 * Copyright (c) 2022 Ken Jiang
 * Released under the MIT license
 * 
 * Validation extend jQuery with 3 functions (thus jQuery elements can perform this 3 functions):
 *     $.extend($.fn , {
 *         validate(): install (new) validator object or get it if already installed;
 *         valid(): excute validate action;
 *         rules(): add validating rules })
 * $.Validator is the key object and main method defined in $.Validator.prototype.
 * 
 * This Extension override $.fn.valid(), $.Validator.prototype.form(), and $.Validator.prototype.elements to support:
 *    use "validate-pack" tag on html elements to divide form elements into groups,
 *    then use "validate-pack" on submit button or $.fn.valid() parameter to specify the group to be validate.
 * * 
 */
(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(["jquery", "../jquery.validate"], factory);
    } else if (typeof module === "object" && module.exports) {
        module.exports = factory(require("jquery"));
    } else {
        factory(jQuery);
    }
}(function ($) {

    $.extend($.validator.defaults, { ignore: "" });  //override ignore: "hidden"

    $.validator.methods.url = function (value, element) {
        return this.optional(element) || /^(?:(?:(?:https?|ftp):)?\/\/)(?:(?:[^\]\[?\/<~#`!@$^&*()+=}|:";',>{ ]|%[0-9A-Fa-f]{2})+(?::(?:[^\]\[?\/<~#`!@$^&*()+=}|:";',>{ ]|%[0-9A-Fa-f]{2})*)?@)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:localhost)|(?:(?:[a-z0-9\u00a1-\uffff][a-z0-9\u00a1-\uffff_-]{0,62})?[a-z0-9\u00a1-\uffff]\.)+(?:[a-z\u00a1-\uffff]{2,}\.?))(?::\d{2,5})?(?:[/?#]\S*)?$/i.test(value);
    }

    $.extend($.fn, {
        __valid__: $.fn.valid,

        valid: function (vpack) {
            var validator = $(this[0]).is("form") ? this.validate() : $(this[0].form).validate();

            validator.validatePack = vpack;  //save the validatePack value passed by parameter.
            return this.__valid__();
        }
    });

    $.extend($.validator.prototype, {
        __form__: $.validator.prototype.form,

        form: function () {
            if (this.submitButton) {
                this.validatePack = $(this.submitButton).attr("validate-pack");  //save the submit button's validatePack value
            }

            return this.__form__();
        },

        __elements__: $.validator.prototype.elements,

        elements: function () {
            var validatePack = this.validatePack;  //get saved validatePack value

            var elements = this.__elements__()  //get elements by save elements() method
                .filter(function () {
                    return $(this).attr("validate-pack") == validatePack;  //filtered by validatePack value
                });

            return elements;
        }
    })

    return $;
}));