define(function (require, exports, module) {

    //TODO: using [ko.dependentObservable] or [ko.computed] replaces the below [subscribe]

    CustomerViewModel = {

        CustomerID: ko.observable(''),
        CompanyName: ko.observable(''),
        ContactName: ko.observable(''),
        ContactTitle: ko.observable(''),
        Address: ko.observable(''),
        City: ko.observable(''),
        Region: ko.observable(''),
        PostalCode: ko.observable(''),
        Country: ko.observable(''),
        Phone: ko.observable(''),
        Fax: ko.observable(''),

        reset: function () {

            this.CustomerID = ko.observable('');
            this.CompanyName = ko.observable('');
            this.ContactName = ko.observable('');
            this.ContactTitle = ko.observable('');
            this.Address = ko.observable('');
            this.City = ko.observable('');
            this.Country = ko.observable('');
            this.Region = ko.observable('');
            this.PostalCode = ko.observable('');
            this.Phone = ko.observable('');
            this.Fax = ko.observable('');
        },

        extend: function () {   //Extends viewmodel properties & functions dynamically

            CustomerViewModel.cities = ko.observableArray();
            CustomerViewModel.countries = ko.observableArray();
            CustomerViewModel.Country.subscribe(function (newValue) {

                //Whenever the country changed, reset & reload related cities
                CustomerViewModel.cities([]);

                if (newValue)
                    Customer.loadCities(newValue, function (data) {

                        CustomerViewModel.cities(data);
                    });
            });
        },

        unmap: function () {

            if(this.cities)
                delete this.cities;
            if(this.countries)
                delete this.countries;
        },
        toModel: function () {

            return ko.viewmodel.toModel(this)
        },

        fromData: function (data) {

            var viewModel = ko.viewmodel.fromModel(data);
            CustomerViewModel = $.extend({}, this, viewModel);
        }

    }

    ModalMode = {
        ADD_CUSTOMER: 'add-customer',
        EDIT_CUSTOMER: 'edit-customer'
    };

    Customer = {

        $add_customer: null,
        $remove_customer: null,

        $customer_modal: null,
        $customer_form: null,

        $alert_container: null,

        settings: {
            addSelector: null,
            removeSelector: null,
            modalSelector: null,
            formSelector: null,
            alertCtnSelector: null,
            cityDataUrl: null,
            countryDataUrl: null,
            customerSaveUrl: null,
            customerUpdateUrl: null,
            customerRemoveUrl: null,
            customerReadUrl: null,
        },

        init: function (options) {

            this.settings = $.extend({}, this.settings, options);

            this.$add_customer = $(this.settings.addSelector);
            this.$remove_customer = $(this.settings.removeSelector);

            this.$add_customer.unbind('click');
            this.$add_customer.bind('click', this.add);
            this.$remove_customer.unbind('click');
            this.$remove_customer.bind('click', this.remove);

            this.$customer_modal = $(this.settings.modalSelector);
            this.$customer_form = $(this.settings.formSelector);
            this.$alert_container = $(this.settings.alertCtnSelector);

            return this;
        },


        /*  Actions */

        add: function () {

            Customer.resetForm();

            CustomerViewModel.reset();
            //Extends customer viewmodel properties dynamically
            CustomerViewModel.extend();

            Customer.loadCountries(function (data) {
                CustomerViewModel.countries(data);
            });

            //Apply viewmodel data for customer form
            Customer.applyData();

            Customer.showModal(ModalMode.ADD_CUSTOMER, 'Add Customer', 'Save');
        },
        doAdd: function () {

            if (!Customer.isValid()) return;

            //Remove extended properties (OPTIONAL)
            CustomerViewModel.unmap();

            var data = CustomerViewModel.toModel();                      
            Customer.postCustomer(data, ModalMode.ADD_CUSTOMER,
                function (result) {

                    Customer.hideModal();
                    CustomerViewModel.reset();
                    CustomerGridView.reload();
                    Customer.alertMessage('success', 'Well done', 'You have added the customer(' + result.CustomerID + ') successfully!');
                },
                function (error) {

                    Customer.hideModal();
                    CustomerViewModel.reset();
                });

        },

        edit: function (id) {

            Customer.resetForm();
            Customer.loadCustomer(id,
                function (data) {

                    //Transforms datamodel to viewmodel
                    CustomerViewModel.fromData(data);
                    //Extends customer viewmodel properties dynamically
                    CustomerViewModel.extend();

                    //Apply viewmodel data for customer form
                    Customer.applyData();
                    Customer.loadCountries(function (data) {

                        var selectedCity = CustomerViewModel.City();
                        var selectedCountry = CustomerViewModel.Country();

                        CustomerViewModel.countries(data);
                        CustomerViewModel.Country(selectedCountry);

                        Customer.loadCities(selectedCountry, function (data) {
                            CustomerViewModel.cities(data);
                            CustomerViewModel.City(selectedCity);
                        });
                    });
                },
                function (error) { });

            Customer.showModal(ModalMode.EDIT_CUSTOMER, 'Edit Customer', 'Save Changes');

        },
        doEdit: function () {

            if (!Customer.isValid()) return;

            //Remove extended properties (OPTIONAL)
            CustomerViewModel.unmap();

            var data = CustomerViewModel.toModel();
            Customer.postCustomer(data, ModalMode.EDIT_CUSTOMER,
                function (result) {

                    Customer.hideModal();
                    CustomerViewModel.reset();
                    CustomerGridView.reload();
                    Customer.alertMessage('success', 'Well done', 'You have saved the customer(' + result.CustomerID + ') successfully!');
                },
                function (error) {

                    Customer.hideModal();
                    CustomerViewModel.reset();
                });
        },

        remove: function (id) {

            var rowIds = CustomerGridView.getSelectedRows();
            if (!rowIds || rowIds.length == 0) {
                bootbox.alert({
                    title: 'Warning',
                    message: "Please at least select a customer!"
                });
                return;
            }

            Customer.onConfirmRemove(rowIds);
        },
        singleRemove: function (id) {

            Customer.onConfirmRemove(new Array(id));
        },
        doRemove: function (ids) {

            Customer.removeCustomer(ids,
                function (data) {

                    CustomerGridView.reload();
                    Customer.alertMessage('success', 'Well done', 'You have removed the selected customer(s) successfully!');
                }, function (error) { });
        },

        showModal: function (mode, title, btnText) {

            this.$customer_modal.find('.modal-title').text(title);
            this.$customer_modal.find('.modal-footer .btn-primary').text(btnText);
            this.$customer_modal.modal('show');

            var btn = this.$customer_modal.find('.modal-footer .btn-primary');
            btn.unbind('click');

            switch (mode) {
                case ModalMode.ADD_CUSTOMER:
                    btn.bind('click', this.doAdd);
                    break;
                case ModalMode.EDIT_CUSTOMER:
                    btn.bind('click', this.doEdit);
                    break;
            }
        },
        hideModal: function () {

            this.$customer_modal.modal('hide');
        },

        /* 
         @type [success | info | warning | danger] */
        alertMessage: function (type, title, message) {

            var html = '<div class="alert alert-' + type + ' alert-message" role="alert">'
                            + '<button type="button" class="close" data-dismiss="alert">x</button>'
                            + '<strong>' + title + ' </strong>  ' + message + ''
                       + '</div>';

            this.$alert_container.prepend(html);
            window.setTimeout(function () {
                $(".alert-message").fadeTo(1500, 0).slideUp(500, function () {
                    $(this).remove();
                });
            }, 2000);
        },

        isValid: function () {

            return this.$customer_form.valid();
        },
        resetForm: function () {

            var validator = $.data(this.$customer_form[0], 'validator')
            if (validator) validator.resetForm();
        },

        /*  Data Actions */

        postCustomer: function (data, mode, successCallback, failCallback) {

            var postUrl = this.settings.customerSaveUrl;

            switch (mode) {
                case ModalMode.ADD_CUSTOMER:
                    postUrl = this.settings.customerSaveUrl;
                    break;
                case ModalMode.EDIT_CUSTOMER:
                    postUrl = this.settings.customerUpdateUrl;
                    break;
            }

            $.ajax({
                url: postUrl,
                type: 'POST',
                data: data,
                dataType: 'json'
            }).done(function (data) {
                if (data)
                    successCallback(data);
            }).fail(function (jqx, textStatus, error) {
                failCallback(error);
                Customer.onPostError(jqx, textStatus, error);
            });
        },
        loadCustomer: function (id, successCallback, failCallback) {

            $.ajax({
                url: this.settings.customerReadUrl,
                type: 'POST',
                data: { customerID: id },
                dataType: 'json'
            }).done(function (data) {
                if (data)
                    successCallback(data);
            }).fail(function (jqx, textStatus, error) {
                failCallback(error);
                Customer.onPostError(jqx, textStatus, error);
            });
        },
        removeCustomer: function (ids, successCallback, failCallback) {

            $.ajax({
                url: this.settings.customerRemoveUrl,
                type: 'POST',
                traditional: true,
                data: { customerIds: ids },
                dataType: 'json'
            }).done(function (data) {
                if (data)
                    successCallback(data);
            }).fail(function (jqx, textStatus, error) {
                failCallback(error);
                Customer.onPostError(jqx, textStatus, error);
            });
        },

        loadCountries: function (successCallback) {

            $.ajax({
                url: this.settings.countryDataUrl,
                type: 'POST',
                data: {},
                dataType: 'json'
            }).done(function (data) {
                if (data)
                    successCallback(data);
            }).fail(function (jqx, textStatus, error) {
                Customer.onPostError(jqx, textStatus, error);
            });
        },
        loadCities: function (country, successCallback) {

            $.ajax({
                url: this.settings.cityDataUrl,
                type: 'POST',
                data: { country: (country || '') },
                dataType: 'json'
            }).done(function (data) {
                if (data)
                    successCallback(data);
            }).fail(function (jqx, textStatus, error) {
                Customer.onPostError(jqx, textStatus, error);
            });
        },

        onConfirmRemove: function (rowIds) {

            bootbox.confirm({
                title: 'Confirm Delete',
                message: "Are you sure to delete customer(s)?",
                callback: function (result) {
                    if (result)
                        Customer.doRemove(rowIds);
                }
            });
        },
        onPostError: function (jqx, textStatus, error) {
            bootbox.dialog({
                message: error,
                title: "Server Error",
                buttons: {
                    ok: {
                        label: "Ok",
                        className: "btn-danger",
                        callback: function () { }
                    }
                }
            });
        },

        /*  Data Bingding */

        applyData: function () {

            var form = Customer.$customer_form[0];
            ko.cleanNode(form);
            ko.applyBindings(CustomerViewModel, form);
        }

    }


    CustomerSearchViewModel = {

        cities: ko.observableArray(),
        countries: ko.observableArray(),

        selectedCity: ko.observable(''),
        selectedCountry: ko.observable(''),

        reset: function () {

            this.cities = ko.observableArray();
            this.countries = ko.observableArray();
            this.selectedCity = ko.observable('');
            this.selectedCountry = ko.observable('');
        },

        extend: function () {

            CustomerSearchViewModel.selectedCountry.subscribe(function (newValue) {

                if (!CustomerGridView.firstLoaded)
                    return;

                //Whenever the country changed, reset & reload related cities
                CustomerSearchViewModel.cities([]);

                if (newValue)
                    Customer.loadCities(newValue, function (data) {

                        CustomerSearchViewModel.cities(data);
                        CustomerGridView.reload();
                    });
                else
                    CustomerGridView.reload();

            });
            this.selectedCity.subscribe(function (newValue) {

                if (!CustomerGridView.firstLoaded)
                    return;

                if (CustomerSearchViewModel.selectedCountry())
                    CustomerGridView.reload();
            });
        }
    }




    CustomerGridView = {

        $grid: null,
        firstLoaded: false,//NOTE:This is temporary solution!!!

        settings: {
            selector: null,
            dataUrl: null
        },

        init: function (options) {

            this.settings = $.extend({}, this.settings, options);
            this.$grid = $(this.settings.selector);

            return this;
        },

        load: function () {

            this.firstLoaded = false;
            var searchHtml = '<div class="col-sm-6 form-inline" id="seach_ex_part">'
                                    + '<div class="form-group">'
                                        + '<label for="search_country_filter">Country: &nbsp; </label>'
                                        + '<select name="Country" id="search_country_filter" data-bind=\'options: countries, optionsText: "CountryName",optionsValue:"CountryCode", optionsCaption: "Select...", value: selectedCountry\' class="form-control"></select>'
                                    + '</div>'
                                    + '&nbsp; <div class="form-group">'
                                        + '<label for="search_city_filter">City: &nbsp; </label>'
                                        + '<select name="City" id="search_city_filter" data-bind=\'options: cities, optionsText: "CityName", optionsValue: "CityCode", optionsCaption: "Select...", value: selectedCity\' class="form-control"></select>'
                                    + '</div>'
                              + '</div>';

            //bootgrid:http://www.jquery-bootgrid.com/Documentation
            this.$grid.bootgrid({
                ajax: true,
                post: function () {
                    /* To accumulate custom parameter with the request object */
                    return {
                        Country: (CustomerSearchViewModel.selectedCountry() || ''),
                        City: (CustomerSearchViewModel.selectedCity() || ''),
                        _t: Math.floor(Date.now() / 1000)
                    };
                },
                selection: true,
                multiSelect: true,
                rowSelect: true,
                //keepSelection: true,
                rowCount: [5, 10, 25, 50, -1],
                url: this.settings.dataUrl,
                formatters: {
                    "commands": function (column, row) {
                        return "<button type=\"button\" class=\"btn  btn-default command-edit\" data-row-id=\"" + row.CustomerID + "\"><span class=\"glyphicon glyphicon-edit\"></span></button> " +
                            "<button type=\"button\" class=\"btn btn-default command-delete\" data-row-id=\"" + row.CustomerID + "\"><span class=\"glyphicon glyphicon-remove\"></span></button>";
                    },
                    "companLink": function (column, row) {
                        return "<a href=\"#\">" + row.CompanyName + "</a>";
                    }
                },
                templates: {
                    header: "<div id=\"{{ctx.id}}\" class=\"{{css.header}}\"><div class=\"row\">" + searchHtml + "<div class=\"col-sm-6 actionBar\"><p class=\"{{css.search}}\"></p><p class=\"{{css.actions}}\"></p></div></div></div>",
                    search: "<div class=\"{{css.search}}\"><div class=\"input-group\"><span class=\"{{css.icon}} input-group-addon {{css.iconSearch}}\"></span> <input type=\"text\" class=\"{{css.searchField}}\" placeholder=\"{{lbl.search}}\" /></div></div>",
                    paginationItem: "<li class=\"{{ctx.css}}\"><a onclick=\"return false;\" href=\"{{ctx.uri}}\" class=\"{{css.paginationButton}}\">{{ctx.text}}</a></li>"
                },
            }).on("initialized.rs.jquery.bootgrid", function () {
                //alert('initialized');  
                //TODO: Checking why this event not fired after the table rendered!
            }).on("loaded.rs.jquery.bootgrid", function () {

                //NOTE:
                //We using "firstLoaded" to simulate [initialized] event,
                //Due to the [initialized] event is not fired after the tabled rendered (it maybe a bug or conflict?)
                if (!CustomerGridView.firstLoaded)
                    CustomerGridView.firstLoaded = true;

                CustomerGridView.$grid.find(".command-edit").on("click", function (e) {

                    Customer.edit($(this).data("row-id"));
                }).end().find(".command-delete").on("click", function (e) {

                    Customer.singleRemove($(this).data("row-id"));
                });

                //(Fixed)Adjust wrapper scrollbar 
                //NOTE:If you need kstar scrollbar you can do this,else ignore this.
                refreshCurrentScrolls(true);
            });


            CustomerSearchViewModel.reset();
            CustomerSearchViewModel.extend();

            var search_node = $('#seach_ex_part')[0];
            ko.cleanNode(search_node);
            ko.applyBindings(CustomerSearchViewModel, search_node);

            Customer.loadCountries(function (data) {

                CustomerSearchViewModel.countries(data);
            });

        },

        reload: function () {

            this.$grid.bootgrid("reload");
        },

        search: function () {
            //NOTE:It'll not be triggered if search box is no value
            this.$grid.bootgrid("search");
        },

        appendRow: function (row) {

            this.$grid.bootgrid("append", [row]);
        },

        getSelectedRows: function () {

            return this.$grid.bootgrid("getSelectedRows")
        }
    };


    module.exports = {

        initView: function (gridDataUrl, cityDataUrl, countryDataUrl,
                            customerSaveUrl, customerUpdateUrl, customerRemoveUrl, customerReadUrl) {

            Customer.init({
                addSelector: '#add_customer',
                removeSelector: '#remove_customer',
                modalSelector: '#customer_modal',
                formSelector: '#customer_form',
                alertCtnSelector: '#alert_container',
                cityDataUrl: cityDataUrl,
                countryDataUrl: countryDataUrl,
                customerSaveUrl: customerSaveUrl,
                customerUpdateUrl: customerUpdateUrl,
                customerRemoveUrl: customerRemoveUrl,
                customerReadUrl: customerReadUrl
            });


            CustomerGridView.init({
                dataUrl: gridDataUrl,
                selector: '#customer_grid'
            }).load();
        }
    };


});