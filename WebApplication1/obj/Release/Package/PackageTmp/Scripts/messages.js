(function (root, $) {
    'use strict';

    var dialog = $('<div id="myModal" class="modal fade"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h4 class="modal-title"></h4><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button></div><div class="modal-body" style="min-height:75px;"></div><div class="modal-footer"><div class="row" style="width: 100%;"><div class="col-sm-4"></div><div class="col-sm-8" style="float: left !important;padding-left: 35px;"></div></div></div></div></div></div>'),
        messages = function ($) {
            return {
                alert: function (message, options) {
                    var deferred = $.Deferred(),
                        defaults = {
                            title: 'Message',
                            buttonName: 'Accept',
                            iconClass: 'fa fa-info-circle text-info',
                            type: 'info'
                        };

                    $.extend(defaults, options);

                    switch (defaults.type) {
                        case 'error':
                            defaults.iconClass = 'fa fa-exclamation-circle text-danger';
                            break;
                        case 'info':
                            defaults.iconClass = 'fa fa-info-circle text-info';
                            break;
                        case 'success':
                            defaults.iconClass = 'fa fa-check-circle text-success';
                            break;
                        case 'warning':
                            defaults.iconClass = 'fa fa-exclamation-circle text-warning';
                            break;
                    }

                    // set the content
                    dialog
                        .clone()
                        .find('.modal-title').text(defaults.title).end()
                        .find('.modal-body').html('<div style="padding-left:40px; padding-top:6px; min-height: 30px;"><span class="' + defaults.iconClass + '" style="color: Tomato;font-size: 32px; position: absolute; margin-left: -40px; margin-top: 20px;"></span><p>' + message + '</p></div>').end()
                        .find('.modal-footer .col-sm-8').html('<button type="button" class="btn btn-primary btn-lg" data-dismiss="modal">Accept</button>').end()
                        .appendTo('body')
                        .modal({
                            backdrop: false
                        })
                        .on('hidden.bs.modal', function () {
                            $(this).remove();
                            deferred.resolve();
                        });
                    return deferred.promise();
                },
                alert1: function (message, options) {
                    var deferred = $.Deferred(),
                        defaults = {
                            title: 'Message',
                            buttonName: 'Accept',
                            iconClass: 'fa fa-info-circle text-info',
                            type: 'info'
                        };

                    $.extend(defaults, options);

                    switch (defaults.type) {
                        case 'error':
                            defaults.iconClass = 'fa fa-exclamation-circle text-danger';
                            break;
                        case 'info':
                            defaults.iconClass = 'fa fa-info-circle text-info';
                            break;
                        case 'success':
                            defaults.iconClass = 'fa fa-check-circle text-success';
                            break;
                        case 'warning':
                            defaults.iconClass = 'fa fa-exclamation-circle text-warning';
                            break;
                    }

                    // set the content
                    dialog
                        .clone()
                        .find('.modal-title').text(defaults.title).end()
                        .find('.modal-body').html('<div style="padding-left:40px; padding-top:6px; min-height: 30px;"><span class="' + defaults.iconClass + '" style="color: Tomato;font-size: 32px; position: absolute; margin-left: -40px; margin-top: 20px;"></span><p>' + message + '</p></div>').end()
                        .find('.modal-footer .col-sm-8').html('<button type="button" class="btn btn-primary btn-lg" data-dismiss="modal">Accept</button>').end()
                        .find('button').on('click', function () {

                            //var flag = $(this).index()
                            //if (flag == 0) {
                            window.location.href = "http://svrwebapps.costex.com/PurchasingApp/";
                            //}
                            //else {
                            //    deferred.resolve($(this).index());
                            //    $(this).closest('.modal').modal('hide');
                            //}

                            //deferred.resolve($(this).index());
                            //$(this).closest('.modal').modal('hide');
                        }).end()
                        .appendTo('body')
                        .modal({
                            backdrop: false
                        })
                        .on('hidden.bs.modal', function () {
                            $(this).remove();
                            deferred.resolve();
                        });
                    return deferred.promise();
                },
                confirm: function (message, options) {
                    var deferred = $.Deferred(),
                        defaults = {
                            title: 'Message',
                            buttonLabels: ['Accept','Cancel'],
                            //iconClass: 'glyphicon glyphicon-info-sign text-info',
                            iconClass: 'fa fa-info-circle text-info',
                            type: 'info'
                        };

                    $.extend(defaults, options);

                    //set buttons
                    var buttons = [];
                    $.each(defaults.buttonLabels, function (index) {
                        //var buttonType = defaults.buttonLabels.length == index + 1 ? 'btn-primary' : 'btn-default';
                        var buttonType = defaults.buttonLabels.length == index + 1 ? 'btn-danger' : 'btn-success';
                        buttons.push('<button class="btn ' + buttonType + '">' + this + '</button>');
                    });

                    // set the content
                    dialog
                        .clone()
                        .find('.modal-title').text(defaults.title).end()
                        .find('.modal-body').html('<div style="padding-left:40px; padding-top:13px; min-height: 30px;"><span class="' + defaults.iconClass + '" style="font-size: 32px; position: absolute; margin-left: -40px; margin-top: 20px;"></span>' + message + '</div>').end()
                        .find('.modal-footer').html(buttons.join('')).end()
                        .find('button').on('click', function () {

                            var flag = $(this).index()
                            if (flag == 0) {
                                window.location.href = "http://svrwebapps.costex.com/PurchasingApp/";                                
                            }
                            else {
                                deferred.resolve($(this).index());
                                $(this).closest('.modal').modal('hide');
                            }
                            
                            //deferred.resolve($(this).index());
                            //$(this).closest('.modal').modal('hide');
                        }).end()
                        .appendTo('body')
                        .modal({
                            backdrop: false
                        })
                        .on('hidden.bs.modal', function () {
                            $(this).remove();
                        });
                    return deferred.promise();
                },
                confirm1: function (message, options) {
                    var deferred = $.Deferred(),
                        defaults = {
                            title: 'Message',
                            buttonLabels: ['Accept', 'Cancel'],
                            //iconClass: 'glyphicon glyphicon-info-sign text-info',
                            iconClass: 'fa fa-info-circle text-info',
                            type: 'info'
                        };

                    $.extend(defaults, options);

                    //set buttons
                    var buttons = [];
                    $.each(defaults.buttonLabels, function (index) {
                        //var buttonType = defaults.buttonLabels.length == index + 1 ? 'btn-primary' : 'btn-default';
                        var buttonType = defaults.buttonLabels.length == index + 1 ? 'btn-danger' : 'btn-success';
                        buttons.push('<button class="btn ' + buttonType + '">' + this + '</button>');
                    });

                    // set the content
                    dialog
                        .clone()
                        .find('.modal-title').text(defaults.title).end()
                        .find('.modal-body').html('<div style="padding-left:40px; padding-top:13px; min-height: 30px;"><span class="' + defaults.iconClass + '" style="font-size: 32px; position: absolute; margin-left: -40px; margin-top: 20px;"></span>' + message + '</div>').end()
                        .find('.modal-footer').html(buttons.join('')).end()
                        .find('button').on('click', function () {

                            var flag = $(this).index()
                            if (flag == 0) {
                                $(this).closest('.modal').modal('hide');
                                ShowAssignUser();
                                //window.location.href = "http://svrwebapps.costex.com/PurchasingApp/";
                            }
                            else {
                                deferred.resolve($(this).index());
                                $(this).closest('.modal').modal('hide');
                                ForcePostBack()
                            }

                            //deferred.resolve($(this).index());
                            //$(this).closest('.modal').modal('hide');
                        }).end()
                        .appendTo('body')
                        .modal({
                            backdrop: false
                        })
                        .on('hidden.bs.modal', function () {
                            $(this).remove();
                        });
                    return deferred.promise();
                },
                notification: function (message, options) {
                    var defaults = {
                        type: 'info'
                    },
                        $notification,
                        notification;
                    $.extend(defaults, options);
                    if ($('#messages-notification').length == 0) {
                        $notification = $('<div id="messages-notification" />').appendTo('body');
                        notification = $notification.kendoNotification({
                            stacking: "down",
                            position: {
                                top: 10
                            }
                        }).data("kendoNotification");
                    } else {
                        $notification = $("#messages-notification");
                        notification = $notification.data("kendoNotification");
                    }

                    notification.show(message, defaults.type);
                },
                progress: function (options) {
                    var defaults = {
                        message: '',
                        show: true
                    };
                    $.extend(defaults, options);
                    if (defaults.show) {
                        if ($('#messages-progress').length == 0) {
                            $('body').append('<div id="messages-progress"><div class="ic-loading"></div></div>');
                        }
                    } else {
                        $('#messages-progress').fadeOut(function () {
                            $(this).remove();
                        });
                    }
                },
                prompt: function (message, options) {
                    var deferred = $.Deferred(),
                        defaults = {
                            title: 'Mensaje',
                            buttonLabels: ['Cancelar', 'Aceptar'],
                            defaultText: ''
                        };

                    $.extend(defaults, options);

                    //set buttons
                    var buttons = [];
                    $.each(defaults.buttonLabels, function (index) {
                        var buttonType = defaults.buttonLabels.length == index + 1 ? 'btn-primary' : 'btn-default';
                        buttons.push('<button class="btn ' + buttonType + '">' + this + '</button>');
                    });

                    // set the content
                    dialog
                        .clone()
                        .find('.modal-title').text(defaults.title).end()
                        .find('.modal-body').html('<div class="form-group" style="margin:0;"><div class="control-label">' + message + '</div><input id="messages-prompt-input" type="text" class="form-control" value="' + defaults.defaultText + '" /></div>').end()
                        .find('.modal-footer').html(buttons.join('')).end()
                        .find('button').on('click', function () {
                            deferred.resolve({
                                input: $(this).closest('.modal').find('#messages-prompt-input').val(),
                                buttonIndex: $(this).index()
                            });
                            $(this).closest('.modal').modal('hide');
                        }).end()
                        .appendTo('body')
                        .modal({
                            backdrop: false
                        })
                        .on('hidden.bs.modal', function () {
                            $(this).remove();
                        });
                    return deferred.promise();
                }
            };
        };

    if (typeof define === 'function' && define.amd) {
        // AMD.
        define(['jquery', 'bootstrap'], messages);
    } else {
        // Browser globals.
        root.messages = messages($);
    }
})(window, jQuery);