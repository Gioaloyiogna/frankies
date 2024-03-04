//using AhercodeWebAPI.HR.Data;
using AhercodeWebAPI.HR.Controllers;
using AhercodeWebAPI.HR.Dtos.Billing;
using AhercodeWebAPI.HR.Dtos.Booking;
using AhercodeWebAPI.HR.Dtos.Company;
using AhercodeWebAPI.HR.Dtos.Currency;
using AhercodeWebAPI.HR.Dtos.Guest;
using AhercodeWebAPI.HR.Dtos.GuestService;
using AhercodeWebAPI.HR.Dtos.HouseKeeping;
using AhercodeWebAPI.HR.Dtos.Notes;
using AhercodeWebAPI.HR.Dtos.PaymentNote;
using AhercodeWebAPI.HR.Dtos.PaymentMethod;
using AhercodeWebAPI.HR.Dtos.PaymentNote;
using AhercodeWebAPI.HR.Dtos.Room;
using AhercodeWebAPI.HR.Dtos.ServiceCategory.cs;
using AhercodeWebAPI.HR.Dtos.ServiceDetails;
using AhercodeWebAPI.HR.Dtos.Tax;
using AhercodeWebAPI.HR.Dtos.User;
using AhercodeWebAPI.HR.UserModels;
using AutoMapper;
using AhercodeWebAPI.HR.Dtos.AuditTrail;

namespace AhercodeWebAPI.HR.Helpers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<UserCreateDto, User>().ReverseMap();
            CreateMap<UserPutDto, User>().ReverseMap();
            CreateMap<UserRoleCreateDto, Role>().ReverseMap();
            CreateMap<TaxCreateDto, TaxTable>().ReverseMap();
            CreateMap<TaxPutDto, TaxTable>().ReverseMap();

            //CreateMap<RoomCategoryCreateDto, Room>().ReverseMap();
            CreateMap<BookingCreateDto, Booking>().ReverseMap();
            CreateMap<NewGuestBookingCreateDto, Booking>().ReverseMap();
            CreateMap<BookingPutDto, Booking>().ReverseMap();

            CreateMap<CurrencyCreateDto, Currency>().ReverseMap();
            CreateMap<CurrencyPutDto, Currency>().ReverseMap();

            CreateMap<CompanyCreateDto, Company>().ReverseMap();
            CreateMap<CompanyPutDto, Company>().ReverseMap();

            CreateMap<PaymentMethodCreateDto, PaymentMethod>().ReverseMap();
            CreateMap<PaymentMethodPutDto, PaymentMethod>().ReverseMap();

            CreateMap<PaymentNoteCreateDto, PaymentNote>().ReverseMap();
            CreateMap<PaymentNotePutDto, PaymentNote>().ReverseMap();

            CreateMap<BillingCreateDto, Billing>().ReverseMap();
            CreateMap<BillingCreateDto, BillingsHistory>().ReverseMap();
            CreateMap<RoomTypeCreateDto, RoomType>().ReverseMap();
            CreateMap<RoomDto, Room>().ReverseMap();

            CreateMap<GuestDto, Guest>().ReverseMap();
            CreateMap<GuestCreateDto, Guest>().ReverseMap();
            CreateMap<NewGuestBookingCreateDto, Guest>().ReverseMap();

            CreateMap<NoteDto, Note>().ReverseMap();
            CreateMap<NoteCreateDto, Note>().ReverseMap();

            CreateMap<ServiceCreateDto, ServiceCategory>().ReverseMap();
            CreateMap<ServiceDetailDto, ServiceDetail>().ReverseMap();
            CreateMap<HouseKeepingItemDto, HouseKeepingItem>().ReverseMap();
            CreateMap<GuestServiceDto, GuestService>().ReverseMap();
            CreateMap<AuditTrailCreateDto, AuditTrail>().ReverseMap();


        }
    }
}
