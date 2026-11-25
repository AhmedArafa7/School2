using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.Admin.TermStartDate
{
    public record PutTermStartDateCommand(DateOnly TermStartDate) : IRequest<Result<DateOnly>>;
    public class PutTermStartDateValidator : AbstractValidator<PutTermStartDateCommand>
    {
        public PutTermStartDateValidator()
        {

        }
    }

    public class PutTermStartDateHandler : IRequestHandler<PutTermStartDateCommand, Result<DateOnly>>
    {
        private readonly ApplicationDbContext _context;

        public PutTermStartDateHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DateOnly>> Handle(PutTermStartDateCommand request, CancellationToken cancellationToken)
        {
            var setting = await _context.AcademicSettings
            .FirstOrDefaultAsync(cancellationToken);

            if (setting is null)
            {
                setting = new AcademicSetting
                {
                    TermStartDate = request.TermStartDate
                };
                await _context.AcademicSettings.AddAsync(setting, cancellationToken);
            }
            else
            {

                if (setting.TermStartDate == request.TermStartDate)
                    return Error.Validation("AcademicSettings.NoChanges", "No changes were made to the term start date.");

                setting.TermStartDate = request.TermStartDate;
            }

            await _context.SaveChangesAsync();
            return setting.TermStartDate;
        }

    }

}

