 
 

CustomerModelViewModel={

	CustomerID :ko.observable(''),
	CompanyName :ko.observable(''),
	ContactName :ko.observable(''),
	ContactTitle :ko.observable(''),
	Address :ko.observable(''),
	City :ko.observable(''),
	Region :ko.observable(''),
	PostalCode :ko.observable(''),
	Country :ko.observable(''),
	Phone :ko.observable(''),
	Fax :ko.observable(''),

	reset:function(){	//Resets viewmodel properties

		CustomerID  = ko.observable('');
		CompanyName  = ko.observable('');
		ContactName  = ko.observable('');
		ContactTitle  = ko.observable('');
		Address  = ko.observable('');
		City  = ko.observable('');
		Region  = ko.observable('');
		PostalCode  = ko.observable('');
		Country  = ko.observable('');
		Phone  = ko.observable('');
		Fax  = ko.observable('');
	},

	extend:function(){	//Extends viewmodel properties & functions dynamically
	
	},

	 unmap: function () {	//Removes dynamic viewmodel properties

     },
     toModel: function () {	//Converts viewmodel to javascript plain object

         return ko.viewmodel.toModel(this)
     },

     fromData: function (data) {	//Transforms json data object to viewmodel

         var viewModel = ko.viewmodel.fromModel(data);
         CustomerModelViewModel = $.extend({}, this, viewModel);
     }
 }

