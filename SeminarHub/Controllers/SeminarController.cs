using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Data.Models;
using SeminarHub.Models;
using System.Globalization;
using System.Security.Claims;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext data;

        public SeminarController(SeminarHubDbContext context)
        {
            this.data = context;
        }
        public async Task<IActionResult> All()
        {
            var model = await this.data.Seminars
                .Select(s => new SeminarInfoViewModel
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    DateAndTime = s.DateAndTime.ToString(DataConstants.DateFormat),
                    Organizer = s.Organizer.UserName,
                    Category = s.Category.Name
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarFormViewModel();
            model.Categories = await GetCategories();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(SeminarFormViewModel model)
        {
            DateTime dateAndTime = DateTime.Now;


            if (!DateTime.TryParseExact(
                model.DateAndTime,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateAndTime))
            {
                ModelState
                    .AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be: {DataConstants.DateFormat}");
            }


            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();

                return View(model);
            }

            var seminar = new Seminar()
            {
                Id = model.Id,
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = dateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                OrganizerId = GetUserId(),
            };

            await this.data.Seminars.AddAsync(seminar);
            await this.data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var model = await this.data.SeminarParticipants
                .AsNoTracking()
                .Where(s => s.ParticipantId == GetUserId())
                 .Select(s => new SeminarInfoViewModel
                 {
                     Id = s.Seminar.Id,
                     Topic = s.Seminar.Topic,
                     Lecturer = s.Seminar.Lecturer,
                     DateAndTime = s.Seminar.DateAndTime.ToString(DataConstants.DateFormat),
                     Organizer = s.Seminar.Organizer.UserName,
                     Category = s.Seminar.Category.Name
                 })
                 .ToListAsync();

            if (model == null)
                return BadRequest();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var e = await this.data.Seminars
                .Where(e => e.Id == id)
                .Include(e => e.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (e == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (!e.SeminarsParticipants.Any(p => p.ParticipantId == userId))
            {
                e.SeminarsParticipants.Add(new SeminarParticipant()
                {
                    SeminarId = e.Id,
                    ParticipantId = userId
                });

                await this.data.SaveChangesAsync();
            }
            else
            {
                return RedirectToAction(nameof(All));
            }
            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var s = await data.Seminars
                .FindAsync(id);

            if (s == null)
            {
                return BadRequest();
            }

            if (s.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var model = new SeminarFormViewModel()
            {
                Id = s.Id,
                Topic = s.Topic,
                Lecturer = s.Lecturer,
                Details = s.Details,
                DateAndTime = s.DateAndTime.ToString(DataConstants.DateFormat),
                Duration = s.Duration,
                CategoryId = s.CategoryId,
            };

            if (model == null)
                return BadRequest();

            model.Categories = await GetCategories();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SeminarFormViewModel model, int id)
        {
            DateTime dateAndTime = DateTime.Now;


            if (!DateTime.TryParseExact(
                model.DateAndTime,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateAndTime))
            {
                ModelState
                    .AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be: {DataConstants.DateFormat}");
            }


            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();

                return View(model);
            }

            var seminar = await this.data.Seminars.FindAsync(id);

            if (seminar == null)
                return BadRequest();

            seminar.Id = model.Id;
            seminar.Topic = model.Topic;
            seminar.Lecturer = model.Lecturer;
            seminar.Details = model.Details;
            seminar.DateAndTime = dateAndTime;
            seminar.Duration = model.Duration;
            seminar.CategoryId = model.CategoryId;
            seminar.OrganizerId = GetUserId();

            await this.data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Leave(int id)
        {
            var seminar = await data.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (seminar == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            var sp = seminar.SeminarsParticipants
                .FirstOrDefault(sp => sp.ParticipantId == userId);

            if (sp == null)
            {
                return BadRequest();
            }

            seminar.SeminarsParticipants.Remove(sp);

            await this.data.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await data.Seminars
               .FindAsync(id);

            if (s == null)
            {
                return BadRequest();
            }

            if (s.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var model = new SeminarDeleteViewModel()
            {
                Id = s.Id,
                DateAndTime = s.DateAndTime,
                Topic = s.Topic
            };

            if (model == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seminar = await data.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (seminar == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (userId != seminar.OrganizerId)
            {
                return BadRequest();
            }

            this.data.Seminars.Remove(seminar);
            await this.data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.data.Seminars
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SeminarDetailsViewModel
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    DateAndTime = s.DateAndTime.ToString(DataConstants.DateFormat),
                    Organizer = s.Organizer.UserName,
                    Category = s.Category.Name,
                    Duration = s.Duration,
                    Details = s.Details
                })
                .FirstOrDefaultAsync();

            if (model == null)
                return BadRequest();

            return View(model);
        }

        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
        {
            return await this.data.Categories
                .AsNoTracking()
                .Select(t => new CategoryViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
