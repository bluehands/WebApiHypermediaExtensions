﻿using System;
using System.Threading.Tasks;
using CarShack.Domain.Customer;
using Bluehands.Hypermedia.Relations;
using WebApi.HypermediaExtensions.Hypermedia;
using WebApi.HypermediaExtensions.Hypermedia.Actions;
using WebApi.HypermediaExtensions.Hypermedia.Attributes;
using WebApi.HypermediaExtensions.Hypermedia.Links;
using WebApi.HypermediaExtensions.Util.Enum;
using WebApi.HypermediaExtensions.Util.Repository;

namespace CarShack.Hypermedia.Customers
{
    [HypermediaObject(Title = "The Customers API", Classes = new[] { "CustomersRoot" })]
    public class HypermediaCustomersRoot : HypermediaObject
    {
        private readonly ICustomerRepository customerRepository;

        // Add actions:
        // Each ActionType must be unique and a corresponding route must exist so the formatter can look it up.
        // See the CustomersRootController.
        [HypermediaAction(Name = "CreateCustomer", Title = "Request creation of a new Customer.")]
        public HypermediaFunction<CreateCustomerParameters, Task<Customer>> CreateCustomerAction { get; private set; }

        [HypermediaAction(Name = "CreateQuery", Title = "Query the Customers collection.")]
        public HypermediaAction<CustomerQuery> CreateQueryAction { get; private set; }

        public HypermediaCustomersRoot(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;

            CreateCustomerAction = new HypermediaFunction<CreateCustomerParameters, Task<Customer>>(CanCreateCustomer, DoCreateCustomer/*, new CreateCustomerParameters{Name = "John Doe"}*/);
            CreateQueryAction = new HypermediaAction<CustomerQuery>(new CustomerQuery
            {
                Filter = new CustomerFilter()
                {
                    MinAge = 12
                },
                Pagination = new Pagination()
                {
                    PageOffset = 0,
                    PageSize = 10
                },
                SortBy = new SortParameter<CustomerSortProperties>
                {
                    PropertyName = CustomerSortProperties.Age,
                    SortType = SortTypes.Ascending
                }

            });

            // Add Links:
            var allQuery = new CustomerQuery();
            Links.Add(DefaultHypermediaRelations.Queries.All, new HypermediaObjectQueryReference(typeof(HypermediaCustomerQueryResult), allQuery));

            // This Link uses a reference to a HypermediaObject without actually building it. It Gives the type and the value which is used do identify the Entity.
            // The key will be used while resolving routes in the Formatter.
            // links to the HypermediaCustomer with Id = 1
            Links.Add("BestCustomer", new HypermediaObjectKeyReference(typeof(HypermediaCustomer), 1));

            // Workaround in case a external reference is needed which can not be build by the framework
            Links.Add("GreatSite", new ExternalReference(new Uri("http://www.example.com/")));
        }

        // Will be called to determine if tis action is available at the moment/current state.
        private bool CanNewQuery()
        {
            return true;
        }

        private async Task<Customer> DoCreateCustomer(CreateCustomerParameters arg)
        {
            var customer = CustomerService.CreateRandomCustomer();
            customer.Name = arg.Name;
            await customerRepository.AddEntityAsync(customer).ConfigureAwait(false);

            return customer;
        }

        private bool CanCreateCustomer()
        {
            return true;
        }
    }
}
