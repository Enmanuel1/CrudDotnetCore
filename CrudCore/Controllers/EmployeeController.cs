using CrudCore.AppContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CrudCore.Models;

namespace CrudCore.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm" ] = sortOrder == "Date" ? "date_desc" : "Date" ;
            ViewData["AgeSortParm"] = sortOrder == "Age" ? "age_desc" : "Age";
            ViewData["CurrentFilter"] = searchString;
            var employee = from e in _context.Employees
                           select e;
            if (!String.IsNullOrEmpty(searchString))
            {
                employee = employee.Where(e => e.Name.Contains(searchString) 
                                            || e.Position.Contains(searchString));
            }
            else
            {
                employee = employee.OrderBy(e => e.Name);
            }
            switch (sortOrder)
            {
                case "name_desc":
                    employee = employee.OrderByDescending(e => e.Name);
                    break;
                case "Date":
                    employee = employee.OrderBy(e => e.CreatedAt);
                    break;
                case "date_desc":
                    employee = employee.OrderByDescending(e => e.CreatedAt);
                    break;
                case "Age":
                    employee = employee.OrderBy(e => e.Age);
                    break;
                case "age_desc":
                    employee = employee.OrderByDescending(e => e.Age);
                    break;
                default:
                    employee = employee.OrderBy(e => e.Name);
                    break;
            }

            return View(await employee.AsNoTracking().ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Employee model)
        {
            var employee = await _context.Employees.AddAsync(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.Id == id);
            return View(employee);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Employee model)
        {
            try
            {
                _context.Employees.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.Id == id);
            return View(employee);
        }
        [HttpGet]
         public async Task<IActionResult> Delete(int? id)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.Id == id);
            return View(employee);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.Id == id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
