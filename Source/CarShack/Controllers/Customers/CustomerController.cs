﻿using System.Threading.Tasks;
using CarShack.Domain.Customer;
using CarShack.Hypermedia.Cars;
using CarShack.Hypermedia.Customers;
using CarShack.Util;
using Microsoft.AspNetCore.Mvc;
using WebApi.HypermediaExtensions.ErrorHandling;
using WebApi.HypermediaExtensions.Exceptions;
using WebApi.HypermediaExtensions.JsonSchema;
using WebApi.HypermediaExtensions.WebApi;
using WebApi.HypermediaExtensions.WebApi.AttributedRoutes;
using WebApi.HypermediaExtensions.WebApi.ExtensionMethods;

namespace CarShack.Controllers.Customers
{
    [Route("Customers")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        #region HypermediaObjects
        // Route to the HypermediaCustomer. References to HypermediaCustomer type will be resolved to this route.
        // This RouteTemplate also contains a key, so a RouteKeyProducer can be provided. In this case the RouteKeyProducer
        // could be ommited and KeyAttribute could be used on HypermediaCustomer instead.
        [HttpGetHypermediaObject("{key:int}", typeof(HypermediaCustomer), typeof(CustomerRouteKeyProducer))]
        public async Task<ActionResult> GetEntity(int key)
        {
            try
            {
                var customer = await customerRepository.GetEnitityByKeyAsync(key).ConfigureAwait(false);
                var result = new HypermediaCustomer(customer);
                return Ok(result);
            }
            catch (EntityNotFoundException)
            {
                return this.Problem(ProblemJsonBuilder.CreateEntityNotFound());
            }
        }
        #endregion

        #region Actions
        [HttpPostHypermediaAction("MyFavoriteCustomers", typeof(HypermediaActionCustomerMarkAsFavorite))]
        public async Task<ActionResult> MarkAsFavoriteAction([HypermediaActionParameterFromBody]FavoriteCustomer favoriteCustomer)
        {
            if (favoriteCustomer == null)
            {
                var problem = new ProblemJson
                {
                    Title = $"Can not use provided object of type '{typeof(FavoriteCustomer)}'",
                    Detail = "Json or contained links might be invalid",
                    ProblemType = "WebApi.HypermediaExtensions.Hypermedia.BadActionParameter",
                    StatusCode = 422 // Unprocessable Entity
                };
                return this.UnprocessableEntity(problem);
            }

            try
            {
                var customer = await customerRepository.GetEnitityByKeyAsync(favoriteCustomer.CustomerId).ConfigureAwait(false);
                var hypermediaCustomer = new HypermediaCustomer(customer);
                hypermediaCustomer.MarkAsFavoriteAction.Execute(favoriteCustomer);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return this.Problem(ProblemJsonBuilder.CreateEntityNotFound());
            }
            catch (InvalidLinkException e)
            {
                var problem = new ProblemJson()
                {
                    Title = $"Can not use provided object of type '{typeof(FavoriteCustomer)}'",
                    Detail = e.Message,
                    ProblemType = "WebApi.HypermediaExtensions.Hypermedia.BadActionParameter",
                    StatusCode = 422 // Unprocessable Entity
                };
                return this.UnprocessableEntity(problem);
            }
            catch (CanNotExecuteActionException)
            {
                return this.CanNotExecute();
            }
        }

        [HttpPostHypermediaAction("{key:int}/BuysCar", typeof(HypermediaActionCustomerBuysCar))]
        public async Task<ActionResult> BuyCar(int key, HypermediaActionCustomerBuysCar.Parameter parameter)
        {
            if (parameter == null)
            {
                var problem = new ProblemJson
                {
                    Title = $"Can not use provided object of type '{typeof(HypermediaActionCustomerBuysCar.Parameter)}'",
                    Detail = "Json or contained links might be invalid",
                    ProblemType = "WebApi.HypermediaExtensions.Hypermedia.BadActionParameter",
                    StatusCode = 422 // Unprocessable Entity
                };
                return this.UnprocessableEntity(problem);
            }

            try
            {
                //shortcut for get car from repository
                var car = new HypermediaCar(parameter.Brand, parameter.CarId);
                var customer = await customerRepository.GetEnitityByKeyAsync(key).ConfigureAwait(false);
                //do what has to be done
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return this.Problem(ProblemJsonBuilder.CreateEntityNotFound());
            }
            catch (CanNotExecuteActionException)
            {
                return this.CanNotExecute();
            }
        }

        [HttpPostHypermediaAction("{key:int}/Moves", typeof(HypermediaActionCustomerMoveAction), typeof(CustomerRouteKeyProducer))]
        public async Task<ActionResult> CustomerMove(int key, NewAddress newAddress)
        {
            if (newAddress == null)
            {
                return this.Problem(ProblemJsonBuilder.CreateBadParameters());
            }

            try
            {
                var customer = await customerRepository.GetEnitityByKeyAsync(key).ConfigureAwait(false);
                var hypermediaCustomer = new HypermediaCustomer(customer);
                hypermediaCustomer.MoveAction.Execute(newAddress);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return this.Problem(ProblemJsonBuilder.CreateEntityNotFound());
            }
            catch (CanNotExecuteActionException)
            {
                return this.CanNotExecute();
            }
            catch (ActionParameterValidationException e)
            {
                var problem = new ProblemJson()
                {
                    Title = $"Can not use provided object of type '{typeof(NewAddress)}'",
                    Detail = e.Message,
                    ProblemType = "WebApi.HypermediaExtensions.Hypermedia.BadActionParameter",
                    StatusCode = 422 // Unprocessable Entity
                };
                return this.UnprocessableEntity(problem);
            }
        }
        #endregion

        #region TypeRoutes
        // Provide type information for Action parameters. Does not depend on a specific customer. Optional when using
        // MvcOptionsExtension.AutoDeliverActionParameterSchemas
        [HttpGetHypermediaActionParameterInfo("NewAddressType", typeof(NewAddress))]
        public async Task<ActionResult> NewAddressType()
        {
            var schema = await JsonSchemaFactory.Generate(typeof(NewAddress)).ConfigureAwait(false);
            return Ok(schema);
        }
        #endregion
    }
}
