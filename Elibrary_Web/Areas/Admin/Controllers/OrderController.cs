using Elibrary.Utility;
using ELibrary.DataAccess.Repository.IRepository;
using ELibrary.Models;
using ELibrary.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Elibrary_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")

            };
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser");

            orderVM.OrderHeader.ApplicationUser.Name = orderHeaderFromDb.ApplicationUser.Name;
            orderVM.OrderHeader.ApplicationUser.PhoneNumber = orderHeaderFromDb.ApplicationUser.PhoneNumber;
            orderVM.OrderHeader.ApplicationUser.StreetAddress = orderHeaderFromDb.ApplicationUser.StreetAddress;
            orderVM.OrderHeader.ApplicationUser.City = orderHeaderFromDb.ApplicationUser.City;
            orderVM.OrderHeader.ApplicationUser.State = orderHeaderFromDb.ApplicationUser.State;
            orderVM.OrderHeader.ApplicationUser.PostalCode = orderHeaderFromDb.ApplicationUser.PostalCode;
            orderVM.OrderHeader.ApplicationUser.Email = orderHeaderFromDb.ApplicationUser.Email;
            orderVM.OrderHeader.OrderDate = orderHeaderFromDb.OrderDate;
            orderVM.OrderHeader.Carrier = orderHeaderFromDb.Carrier;
            orderVM.OrderHeader.TrackingNumber = orderHeaderFromDb.TrackingNumber;
            orderVM.OrderHeader.ShippingDate = orderHeaderFromDb.ShippingDate;
            orderVM.OrderHeader.sessionId = orderHeaderFromDb.sessionId;
            orderVM.OrderHeader.PaymentIntentId = orderHeaderFromDb.PaymentIntentId;
            orderVM.OrderHeader.PaymentDueDate = orderHeaderFromDb.PaymentDueDate;
            orderVM.OrderHeader.PaymentDate = orderHeaderFromDb.PaymentDate;
            orderVM.OrderHeader.PaymentStatus = orderHeaderFromDb.PaymentStatus;

            return View(orderVM);
        }
        public IActionResult UpdateOrderDetails()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            switch (status)
            {

                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }


            return Json(new { data = objOrderHeaders });
        }
        #endregion
    }
}
