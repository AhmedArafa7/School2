using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;

namespace School_VerticalSlice.Features.Admin.TermStartDate
{
    public record GetTermStartDateCommand() : IRequest<Result<DateOnly>>;
    public class GetTermStartDateValidator : AbstractValidator<GetTermStartDateCommand>
    {
        public GetTermStartDateValidator()
        {

        }
    }

    public class GetTermStartDateHandler : IRequestHandler<GetTermStartDateCommand, Result<DateOnly>>
    {
        private readonly ApplicationDbContext _context;

        public GetTermStartDateHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DateOnly>> Handle(GetTermStartDateCommand request, CancellationToken cancellationToken)
        {
            var termStartDate = await _context.AcademicSettings
                .AsNoTracking()
                .Select(a => a.TermStartDate)
                .FirstOrDefaultAsync();

            if (termStartDate == null || termStartDate == default)
                return Error.NotFound("AcademicSettings.NotFound", "Term start date is not configured.");

            return termStartDate;

        }

    }

}

