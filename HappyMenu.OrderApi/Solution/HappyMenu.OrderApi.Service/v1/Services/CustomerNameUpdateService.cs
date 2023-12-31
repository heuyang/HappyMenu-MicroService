﻿using HappyMenu.OrderApi.Service.v1.Command;
using HappyMenu.OrderApi.Service.v1.Models;
using HappyMenu.OrderApi.Service.v1.Query;
using MediatR;
using System;
using System.Diagnostics;

namespace HappyMenu.OrderApi.Service.v1.Services
{
    public class CustomerNameUpdateService : ICustomerNameUpdateService
    {
        private readonly IMediator _mediator;

        public CustomerNameUpdateService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async void UpdateCustomerNameInOrders(UpdateCustomerFullNameModel updateCustomerFullNameModel)
        {
            try
            {
                var ordersOfCustomer = await _mediator.Send(new GetOrderByCustomerGuidQuery
                {
                    CustomerCuid = updateCustomerFullNameModel.Id
                });

                if (ordersOfCustomer.Count != 0)
                {
                    ordersOfCustomer.ForEach(x => x.CustomerFullName = $"{updateCustomerFullNameModel.FirstName} {updateCustomerFullNameModel.LastName}");
                }

                await _mediator.Send(new UpdateOrderCommand
                {
                    Orders = ordersOfCustomer
                });
            }
            catch (Exception ex)
            {
                // log an error message here

                Debug.WriteLine(ex.Message);
            }
        }
    }

}
