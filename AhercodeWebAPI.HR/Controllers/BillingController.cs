using AhercodeWebAPI.HR.UserModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using AhercodeWebAPI.HR.Dtos.Billing;

namespace AhercodeWebAPI.HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : Controller
    {
        private readonly FrankiesHotelContext _context;
        private readonly IMapper mapper;

        public static IWebHostEnvironment _environment;
        public BillingController(IWebHostEnvironment environment, FrankiesHotelContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
            _environment = environment;
        }

        [HttpGet("id")]
        public async Task<ActionResult<IEnumerable<Billing>>> GetBillings(int id,bool isCorporate,string serviceType)
        {
            // Get the current date
            DateTime currentDate = DateTime.Now.Date;

            // Find bookings with null CheckInTime and BookEnd date before today
            List<Billing> guestBilling = new List<Billing>();

            if (isCorporate)
            {
                //if (serviceType.ToLower() == "all_charges")
                //{
                //guestBilling = await _context.Billings.Where(b => b.CompanyId == id).ToListAsync();
                //}else if (serviceType.ToLower() == "rooms_only"){
                //guestBilling = await _context.Billings.Where(b => b.CompanyId == id && b.Description.Contains("Accommodation for Room")).ToListAsync();
                //}
                //else if (serviceType.ToLower() == "debit_notes")
                //{
                //    guestBilling = await _context.Billings.Where(b => b.CompanyId == id && b.Credit==null && (b.IsPayment == false|| b.IsPayment == null) &&b.Debit!=null&&b.RoomId==null).ToListAsync();
                //}
                //else if (serviceType.ToLower() == "credit_notes")
                //{
                //    guestBilling = await _context.Billings.Where(b => b.CompanyId == id && b.Debit==null && (b.IsPayment == false|| b.IsPayment == null) &&b.Credit!=null&&b.RoomId==null).ToListAsync();
                //}
                //else
                //{
                //guestBilling = await _context.Billings.Where(b => b.CompanyId == id && b.Description.Contains("Accommodation for Room")==false&&b.Credit==null&&b.Debit!=null&&(b.IsPayment==false||b.IsPayment==null)).ToListAsync();
                //}
                guestBilling = await _context.Billings
                .Where(b =>
                    b.CompanyId == id &&(
                    (serviceType.ToLower().Contains("all_charges") && b.Description.Contains("Accommodation for Room") == false) ||
                    (serviceType.ToLower().Contains("rooms_only") && b.Description.Contains("Accommodation for Room")) ||
                    (serviceType.ToLower().Contains("debit_notes") && (b.Credit == null && (b.IsPayment == false || b.IsPayment == null) && b.Debit != null && b.RoomId == null)) ||
                    (serviceType.ToLower().Contains("credit_notes") && (b.Debit == null && (b.IsPayment == false || b.IsPayment == null) && b.Credit != null && b.RoomId == null)) ||
                    (serviceType.ToLower().Contains("services_only") && (b.Credit == null && (b.IsPayment == false || b.IsPayment == null) && b.Debit != null && b.Description.Contains("Accommodation for Room") == false) && b.RoomId != null))
                )
                .ToListAsync();
            }
            else
            {
                //if (serviceType.ToLower() == "all_charges")
                //{
                //    guestBilling = await _context.Billings.Where(b => b.CustomerId == id).ToListAsync();
                //}
                //else if (serviceType.ToLower() == "rooms_only")
                //{
                //    guestBilling = await _context.Billings.Where(b => b.CustomerId == id && b.Description.Contains("Accommodation for Room")).ToListAsync();
                //}
                //else if (serviceType.ToLower() == "debit_notes")
                //{
                //    guestBilling = await _context.Billings.Where(b => b.CustomerId == id && b.Credit == null && (b.IsPayment == false || b.IsPayment == null) && b.Debit != null && b.RoomId == null).ToListAsync();
                //}
                //else if (serviceType.ToLower() == "credit_notes")
                //{
                //    guestBilling = await _context.Billings.Where(b => b.CustomerId == id && b.Debit == null && (b.IsPayment == false || b.IsPayment == null) && b.Credit != null && b.RoomId == null).ToListAsync();
                //}
                //else
                //{
                //    guestBilling = await _context.Billings.Where(b => b.CustomerId == id && b.Description.Contains("Accommodation for Room") == false && b.Credit == null && b.Debit != null && (b.IsPayment == false || b.IsPayment == null)).ToListAsync();
                //}
                guestBilling = await _context.Billings
                .Where(b =>
                    b.CustomerId == id && (
                    (serviceType.ToLower().Contains("all_charges")) ||
                    (serviceType.ToLower().Contains("rooms_only") && b.Description.Contains("Accommodation for Room")) ||
                    (serviceType.ToLower().Contains("debit_notes") && (b.Credit == null && (b.IsPayment == false || b.IsPayment == null) && b.Debit != null && b.RoomId == null)) ||
                    (serviceType.ToLower().Contains("credit_notes") && (b.Debit == null && (b.IsPayment == false || b.IsPayment == null) && b.Credit != null && b.RoomId == null)) ||
                    (serviceType.ToLower().Contains("services_only") && (b.Credit == null && (b.IsPayment == false || b.IsPayment == null) && b.Debit != null && b.Description.Contains("Accommodation for Room") == false && b.RoomId != null)))
                )
                .ToListAsync();
                //guestBilling = await _context.Billings.Where(b => b.CustomerId == id).ToListAsync();
            }

            return Ok(guestBilling);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Billing>>> PostBillings(BillingCreateDto billingCreate)
        {
            var billing = mapper.Map<Billing>(billingCreate);
            var billingHistory = mapper.Map<BillingsHistory>(billingCreate);

            await _context.Billings.AddAsync(billing);
            await _context.BillingsHistories.AddAsync(billingHistory);
            await _context.SaveChangesAsync();

            return Ok(billing.Id);
        }
        [HttpPost("BillingTransfer")]
        public async Task<ActionResult<IEnumerable<Billing>>> TransferBillings(BillingTransferCreateDto transferCreate)
        {
            //var billing = mapper.Map<Billing>(billingCreate);
            //var billingHistory = mapper.Map<BillingsHistory>(billingCreate);

            Billing billingToTransfer = new Billing(); 
            Billing billingToTransferSource = new Billing(); 
            BillingsHistory billingHistoryToTransfer = new BillingsHistory();
            BillingsHistory billingHistoryToTransferSource = new BillingsHistory();

            //Crediting bill from source
            billingToTransferSource.CustomerId = transferCreate.customerId;
            billingToTransferSource.Debit = null;
            billingToTransferSource.Credit = transferCreate.amount;
            billingToTransferSource.Currency = transferCreate.currency;
            billingToTransferSource.Description = transferCreate.description;
            billingToTransferSource.Timestamp = DateTime.Now;
            await _context.Billings.AddAsync(billingToTransferSource);
            //await _context.SaveChangesAsync();

            //Crediting bill from source (History)
            billingHistoryToTransferSource.CustomerId = transferCreate.customerId;
            billingHistoryToTransferSource.Credit = transferCreate.amount;
            //billingHistoryToTransfer.Debit = null;
            billingHistoryToTransferSource.Currency = transferCreate.currency;
            billingHistoryToTransferSource.Description = transferCreate.description;
            billingHistoryToTransferSource.Timestamp = DateTime.Now;
            await _context.BillingsHistories.AddAsync(billingHistoryToTransferSource);
            //await _context.SaveChangesAsync();


            //Transfering billing to receiver
            billingToTransfer.CustomerId = transferCreate.receiverId;
            billingToTransfer.CustomerIdTransferedFrom = transferCreate.customerId;
            billingToTransfer.Debit = transferCreate.amount;
            //billingToTransfer.Credit = null;
            billingToTransfer.Currency = transferCreate.currency;
            billingToTransfer.Description = transferCreate.description;
            billingToTransfer.Timestamp = DateTime.Now;
            await _context.Billings.AddAsync(billingToTransfer);
            //await _context.SaveChangesAsync();

            //Transfering billing to receiver (History)
            billingHistoryToTransfer.CustomerId = transferCreate.receiverId;
            billingHistoryToTransfer.CustomerIdTransferedFrom = transferCreate.customerId;
            billingHistoryToTransfer.Debit = transferCreate.amount;
            //billingHistoryToTransfer.Credit = null;
            billingHistoryToTransfer.Currency = transferCreate.currency;
            billingHistoryToTransfer.Description = transferCreate.description;
            billingHistoryToTransfer.Timestamp = DateTime.Now;
            await _context.BillingsHistories.AddAsync(billingHistoryToTransfer);

            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpDelete("id")]
        public async Task<ActionResult<Billing>> DeleteBilling(int id)
        {
            var billing = await _context.Billings.FindAsync(id);
            _context.Billings.Remove(billing);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
