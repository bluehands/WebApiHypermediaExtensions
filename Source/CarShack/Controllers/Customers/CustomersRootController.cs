﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CarShack.Domain.Customer;
using CarShack.Hypermedia.Customers;
using CarShack.Util;
using Microsoft.AspNetCore.Mvc;
using WebApi.HypermediaExtensions.Hypermedia.Actions;
using WebApi.HypermediaExtensions.Hypermedia.Links;
using WebApi.HypermediaExtensions.Util.Repository;
using WebApi.HypermediaExtensions.WebApi.AttributedRoutes;
using WebApi.HypermediaExtensions.WebApi.ExtensionMethods;

namespace CarShack.Controllers.Customers
{
    [Route("Customers/")]
    public class CustomersRootController : Controller
    {
        private readonly HypermediaCustomersRoot customersRoot;
        private readonly ICustomerRepository customerRepository;

        public CustomersRootController(HypermediaCustomersRoot customersRoot, ICustomerRepository customerRepository)
        {
            this.customersRoot = customersRoot;
            this.customerRepository = customerRepository;
        }

#region HypermediaObjects
        // Route to the HypermediaCustomersRoot. References to HypermediaCustomersRoot type will be resolved to this route.
        [HttpGetHypermediaObject("", typeof(HypermediaCustomersRoot))]
        public ActionResult GetRootDocument()
        {
            return Ok(customersRoot);
        }

        // Building Queries using the CreateQuery will link to this route.
        [HttpGetHypermediaObject("Query", typeof(HypermediaCustomerQueryResult))]
        public async Task<ActionResult> Query([FromQuery] CustomerQuery query)
        {
            if (query == null)
            {
                return this.Problem(ProblemJsonBuilder.CreateBadParameters());
            }

            var queryResult = await customerRepository.QueryAsync(query).ConfigureAwait(false);
            var resultReferences = new List<HypermediaObjectReferenceBase>();
            foreach (var customer in queryResult.Entities)
            {
                resultReferences.Add(new HypermediaObjectReference(new HypermediaCustomer(customer)));
            }

            var result = new HypermediaCustomerQueryResult(resultReferences, queryResult.TotalCountOfEnties, query);
            var navigationQuerys = NavigationQuerysBuilder.Build(query, queryResult);
            result.AddNavigationQueries(navigationQuerys);
           
            return Ok(result);
        }
#endregion

#region Actions
        // Provides a link to the result Query.
        [HttpPostHypermediaAction("Queries", typeof(HypermediaAction<CustomerQuery>))]
        public ActionResult NewQueryAction(CustomerQuery query)
        {
            if (query == null)
            {
                return this.Problem(ProblemJsonBuilder.CreateBadParameters());
            }

            if (!customersRoot.CreateQueryAction.CanExecute())
            {
                return this.CanNotExecute();
            }

            // Will create a Location header with a URI to the result.
            return this.CreatedQuery(typeof(HypermediaCustomerQueryResult), query);
        }

        [HttpPostHypermediaAction("CreateCustomer", typeof(HypermediaFunction<CreateCustomerParameters, Task<Customer>>))]
        public async Task<ActionResult> NewCustomerAction(CreateCustomerParameters createCustomerParameters)
        {
            if (createCustomerParameters == null)
            {
                return this.Problem(ProblemJsonBuilder.CreateBadParameters());
            }

            var createdCustomer = await customersRoot.CreateCustomerAction.Execute(createCustomerParameters).ConfigureAwait(false);

            // Will create a Location header with a URI to the result.
            return this.Created(new HypermediaCustomer(createdCustomer));
        }
#endregion
    }
}
