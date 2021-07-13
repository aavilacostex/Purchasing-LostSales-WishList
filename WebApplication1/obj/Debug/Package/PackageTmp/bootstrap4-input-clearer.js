/*
 *  bootstrap4-input-clearer v1.2.0
 *  Add clear icons to Bootstrap 4 input fields.
 *  https://github.com/mheigl/bootstrap4-input-clearer
 *
 *  Made by Michael Heigl
 *  Under GNU General Public License v3.0
 */
(function ($) {
    "use strict";

    var pluginName = "clearer",
        defaults = {
            clearHtml: '&times;',
            cssClass: 'input-clearer',
            focusable: true
        };

    function Plugin(element, options) {
        //alert(element.nodeName);
        //alert(element.tagName);
        this.element = element;
        this.settings = $.extend({}, defaults, options);
        this.init();
    }

    $.extend(Plugin.prototype, {
        init: function () {
            var self = this;

            this.$element = $(this.element);            

            //this.$clearer = $('<a href="#" class="input-group-append ' + this.settings.cssClass + '"><div class="input-group-text">'  + this.settings.clearHtml + '</div></a>');
            this.$clearer = $('<a href="#" class="' + this.settings.cssClass + '"><i class="fa fa-times"></i></a>');
               

            if (this.settings.focusable === false) {
                this.$clearer.attr({ 'tabindex': -1 });
            }

            if (this.$element.closest('.input-group').length === 0) {
                this.$element.wrap("<div class='input-group'></div>");
            }            

            if (this.element.nodeName == "INPUT") {                
                this.$element.after(this.$clearer);
                this.update();                
            }

            if (this.element.nodeName == "SELECT") {

                var id = this.element.id;

                if (id != "MainContent_ddlPageSize" && id != "MainContent_ddlVendAssign" ) {
                    var idd = document.getElementById(id);

                    //console.log(idd);
                    

                    if ($(idd).val() != -1) {

                        if ($(idd).val() != " ") {

                            this.$element.after(this.$clearer);
                            this.update();
                            //console.log(id);
                            //console.log(idd);
                            //console.log($(id).val());
                            //console.log($(idd).val());
                        }                        
                    }
                    else {
                        //console.log("empty");
                    }
                }                
            }

            //this.$element.after(this.$clearer);
            //this.update();            

            this.$clearer.on('click.clearer', function (e) {
                self.$element.val('');
                self.$element.trigger('change');
                self.$element.focus();
                self.update();
                e.preventDefault();
                console.log("1");                
            });

            $('select').on('change', function (e) {                
                //$(this).after(this.$clearer);
                //this.update();
                //var data = $(this).val();
                //alert(data);
                self.$element.after(this.$clearer);
                self.update();
                //self.$element.update();               

                //console.log(self.$element);
                //var peep = $(this.element)
                //var piip = e.target
                //console.log(object.keys(peep))
                //console.log(self);
                //console.log(piip);
                //object.keys(peep)
                //alert(object.keys(peep));
            });

            this.$element.on('focus.clearer blur.clearer', function () {
                self.update();
            });

            this.$element.on('keyup.clearer', function (e) {
                if (e.keyCode === 27) {
                    $(this).val('').focus();
                }

                self.update();
            });

            this.$element.on('input.clearer change.clearer paste.clearer', function () {
                self.update();
            });
        },
        update: function () {
            //console.log(this.$element.val());
            //console.log(this.$element);

            if (this.$element.val() != null) {
                if (this.$element.val().length >= 1) {
                    this.$clearer.show();
                } else {
                    this.$clearer.hide();
                }
            }            
        }
    });

    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, "plugin_" + pluginName)) {
                $.data(this, "plugin_" +
                    pluginName, new Plugin(this, options));
            }
        });
    };
})(jQuery, window, document);