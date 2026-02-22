using AutoMapper;
using LibraryApp.Domain.Entities;
using LibraryApp.Application.Contracts;

namespace LibraryApp.Application.Common;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryListItemDto>();
        CreateMap<Author, AuthorDetailDto>();
        CreateMap<Author, AuthorListItemDto>();

        CreateMap<Book, BookListItemDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            
        CreateMap<Book, BookDetailDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        CreateMap<Member, MemberListItemDto>();
        CreateMap<Member, MemberDetailDto>();

        CreateMap<Loan, LoanListItemDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book!.Title))
            .ForMember(dest => dest.MemberFullName, opt => opt.MapFrom(src => src.Member!.FullName));
            
        CreateMap<Loan, LoanDetailDto>()
            .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
            .ForMember(dest => dest.Member, opt => opt.MapFrom(src => src.Member));
    }
}
