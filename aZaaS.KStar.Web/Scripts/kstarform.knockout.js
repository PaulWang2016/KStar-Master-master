
/** Custom BindingHandlers **/

ko.bindingHandlers.selectedText = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        value($("option:selected", element).text());

        $(element).change(function () {
            value($("option:selected", this).text());
        });
    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        if (value) {
            $("option", element).filter(function (i, el) { return $(el).text() === value; }).prop("selected", "selected");
        }
    }
};

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var defaults = {
            format: "YYYY-MM-DD",
            pickTime:false,
            useMinutes: false,
            changeCallBack: false,
            callBackParameter: null
           
        };
        //changeCallBack: function(currentObservable,callBackParameter,value)
        var options = $.extend({}, defaults, allBindingsAccessor().datepickerOptions);

        var optionName = "";
        var optionObservables = "";
        $.each(options, function (attr, value) {

            if (typeof value == "function" && value.name == "observable" && attr != "callBackParameter") {
                optionObservables = value;
                optionName = attr;
            }
        });
        if (optionName != "") {
            options[optionName] = optionObservables();
            if (optionName == "minDate" || optionName == "maxDate") {
                optionObservables.subscribe(function (newValue) {
                    var element0ptions = $(element).data('DateTimePicker').options;
                    element0ptions[optionName] = moment(newValue);
                    if (optionName == "minDate") {
                        $(element).data('DateTimePicker').setMinDate(newValue)
                    } else if (optionName == "maxDate") {
                        $(element).data('DateTimePicker').setMaxDate(newValue)
                    }
                });
            }
        }

        $(element).datetimepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value(event.date);
                if (typeof options.changeCallBack == 'function') {
                    try{
                        options.changeCallBack(value, options.callBackParameter, event.date); 
                    } catch (e) {

                    } 
                }
            } else { 
                if (typeof options.changeCallBack == 'function') {
                    try {
                        options.changeCallBack(value, options.callBackParameter, event.date);
                    } catch (e) {

                    }
                }
            }
        });
    },
    update: function (element, valueAccessor) {
        var widget = $(element).data("DateTimePicker");
        //when the view model is updated, update the widget
        if (widget) {
            var newValue = ko.utils.unwrapObservable(valueAccessor());
            if (newValue != null && newValue!="")
            widget.setDate(newValue);         
        }
    }
};