using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PEXHub.Models;

namespace PEXHub.Controllers
{
    public class AuthsController : Controller
    {
        private readonly PEXContext _context;

        public AuthsController(PEXContext context)
        {
            _context = context;
        }

        // GET: Auths
        public async Task<IActionResult> Index(string searchFilter="", string routeBag = "")
        {
            //Authenticate
            #region Authenticate
            var username = User.Identity.Name;
            if (username != null) { AuthenticateUser(); }
            #endregion

            #region Process RouteBag v1.0 - Index/Queues
            RouteBag _routeBag = null;
            if (!String.IsNullOrEmpty(routeBag)) //Load routeBag from View
            {
                _routeBag = JsonConvert.DeserializeObject<RouteBag>(routeBag);
                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
            }
            if (_routeBag == null) //if no routeBag is created, then create one (only on Index Pages with queues)
            {
                _routeBag = new RouteBag();
                _routeBag.QueueId = 0;
            }
            if (!String.IsNullOrEmpty(searchFilter)) //if a new search Filter is intered update routeBag
            {
                _routeBag.SearchString = searchFilter;
            }
            ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
            #endregion

            var pEXContext = _context.Auths.Include(a => a.ReportAuth).Include(a => a.ReportConfig);
            return View(await pEXContext.ToListAsync());
        }

        // GET: Auths/Details/5
        public async Task<IActionResult> Details(int? id, string routeBag = "")
        {
            //Authenticate
            #region Authenticate
            var username = User.Identity.Name;
            if (username != null) { AuthenticateUser(); }
            #endregion

            #region Process RouteBag
            RouteBag _routeBag = null;
            if (!String.IsNullOrEmpty(routeBag))
            {
                _routeBag = JsonConvert.DeserializeObject<RouteBag>(routeBag);
                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
            }
            #endregion

            if (id == null)
            {
                return NotFound();
            }

            var auth = await _context.Auths
                .Include(a => a.ReportAuth)
                .Include(a => a.ReportConfig)
                .FirstOrDefaultAsync(m => m.ReportAuthID == id);
            if (auth == null)
            {
                return NotFound();
            }

            return View(auth);
        }

        // GET: Auths/Create
        public IActionResult Create(string routeBag="")
        {
            //Authenticate
            #region Authenticate
            var username = User.Identity.Name;
            if (username != null) { AuthenticateUser(); }
            #endregion

            #region Process RouteBag
            RouteBag _routeBag = null;
            if (!String.IsNullOrEmpty(routeBag))
            {
                _routeBag = JsonConvert.DeserializeObject<RouteBag>(routeBag);
                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
            }
            #endregion

            ViewData["ReportAuthID"] = new SelectList(_context.ReportAuths, "ID", "Description");
            ViewData["ReportConfigID"] = new SelectList(_context.ReportConfigs, "ID", "Name");
            return View();
        }

        // POST: Auths/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportConfigID,ReportAuthID")] Auth auth, string routeBag = "")
        {
            //Authenticate
            #region Authenticate
            var username = User.Identity.Name;
            if (username != null) { AuthenticateUser(); }
            #endregion

            #region Process RouteBag
            RouteBag _routeBag = null;
            if (!String.IsNullOrEmpty(routeBag))
            {
                _routeBag = JsonConvert.DeserializeObject<RouteBag>(routeBag);
                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
            }
            #endregion

            if (ModelState.IsValid)
            {
                _context.Add(auth);
                await _context.SaveChangesAsync();

                #region Router v1.0
                if (_routeBag != null)
                {
                    if (_routeBag.Routes.Count() > 0)
                    {
                        RouteEl routeEl = new RouteEl();

                        switch (_routeBag.Routes.Count())
                        {
                            case 0:
                                return RedirectToAction(nameof(Index));
                            case 1:
                                routeEl = _routeBag.Routes[0]; //list index starts with 0
                                _routeBag.Routes.Remove(routeEl);
                                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
                                break;
                            case 2:
                                routeEl = _routeBag.Routes[1]; //list index starts with 0
                                _routeBag.Routes.Remove(routeEl);
                                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
                                break;
                            default:
                                return RedirectToAction(nameof(Index));
                        }

                        if (routeEl.Id > 0)
                        {
                            return RedirectToAction(routeEl.Action.ToString(), _routeBag.TranslateRoute(routeEl.RoutePath), new { ID = routeEl.Id, ViewBag.routeBag });
                        }
                        else
                        {
                            return RedirectToAction(routeEl.Action.ToString(), _routeBag.TranslateRoute(routeEl.RoutePath), new { ViewBag.routeBag });
                        }
                    }
                }
                #endregion

                return RedirectToAction(nameof(Index));
            }
            ViewData["ReportAuthID"] = new SelectList(_context.ReportAuths, "ID", "Description", auth.ReportAuthID);
            ViewData["ReportConfigID"] = new SelectList(_context.ReportConfigs, "ID", "Name", auth.ReportConfigID);
            return View(auth);
        }

        // GET: Auths/Edit/5
        public async Task<IActionResult> Edit(int? id, string routeBag = "")
        {
            //Authenticate
            #region Authenticate
            var username = User.Identity.Name;
            if (username != null) { AuthenticateUser(); }
            #endregion

            #region Process RouteBag
            RouteBag _routeBag = null;
            if (!String.IsNullOrEmpty(routeBag))
            {
                _routeBag = JsonConvert.DeserializeObject<RouteBag>(routeBag);
                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
            }
            #endregion

            if (id == null)
            {
                return NotFound();
            }

            var auth = await _context.Auths.FindAsync(id);
            if (auth == null)
            {
                return NotFound();
            }
            ViewData["ReportAuthID"] = new SelectList(_context.ReportAuths, "ID", "Description", auth.ReportAuthID);
            ViewData["ReportConfigID"] = new SelectList(_context.ReportConfigs, "ID", "Name", auth.ReportConfigID);
            return View(auth);
        }

        // POST: Auths/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReportConfigID,ReportAuthID")] Auth auth, string routeBag = "")
        {
            //Authenticate
            #region Authenticate
            var username = User.Identity.Name;
            if (username != null) { AuthenticateUser(); }
            #endregion

            #region Process RouteBag
            RouteBag _routeBag = null;
            if (!String.IsNullOrEmpty(routeBag))
            {
                _routeBag = JsonConvert.DeserializeObject<RouteBag>(routeBag);
                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
            }
            #endregion

            if (id != auth.ReportAuthID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auth);
                    await _context.SaveChangesAsync();

                    #region Router v1.0
                    if (_routeBag != null)
                    {
                        if (_routeBag.Routes.Count() > 0)
                        {
                            RouteEl routeEl = new RouteEl();

                            switch (_routeBag.Routes.Count())
                            {
                                case 0:
                                    return RedirectToAction(nameof(Index));
                                case 1:
                                    routeEl = _routeBag.Routes[0]; //list index starts with 0
                                    _routeBag.Routes.Remove(routeEl);
                                    ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
                                    break;
                                case 2:
                                    routeEl = _routeBag.Routes[1]; //list index starts with 0
                                    _routeBag.Routes.Remove(routeEl);
                                    ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
                                    break;
                                default:
                                    return RedirectToAction(nameof(Index));
                            }

                            if (routeEl.Id > 0)
                            {
                                return RedirectToAction(routeEl.Action.ToString(), _routeBag.TranslateRoute(routeEl.RoutePath), new { ID = routeEl.Id, ViewBag.routeBag });
                            }
                            else
                            {
                                return RedirectToAction(routeEl.Action.ToString(), _routeBag.TranslateRoute(routeEl.RoutePath), new { ViewBag.routeBag });
                            }
                        }
                    }
                    #endregion
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthExists(auth.ReportAuthID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReportAuthID"] = new SelectList(_context.ReportAuths, "ID", "Description", auth.ReportAuthID);
            ViewData["ReportConfigID"] = new SelectList(_context.ReportConfigs, "ID", "Name", auth.ReportConfigID);
            return View(auth);
        }

        // GET: Auths/Delete/5
        public async Task<IActionResult> Delete(int? id, string routeBag = "")
        {
            //Authenticate
            #region Authenticate
            var username = User.Identity.Name;
            if (username != null) { AuthenticateUser(); }
            #endregion

            #region Process RouteBag
            RouteBag _routeBag = null;
            if (!String.IsNullOrEmpty(routeBag))
            {
                _routeBag = JsonConvert.DeserializeObject<RouteBag>(routeBag);
                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
            }
            #endregion

            if (id == null)
            {
                return NotFound();
            }

            var auth = await _context.Auths
                .Include(a => a.ReportAuth)
                .Include(a => a.ReportConfig)
                .FirstOrDefaultAsync(m => m.ReportAuthID == id);
            if (auth == null)
            {
                return NotFound();
            }

            return View(auth);
        }

        // POST: Auths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string routeBag = "")
        {
            //Authenticate
            #region Authenticate
            var username = User.Identity.Name;
            if (username != null) { AuthenticateUser(); }
            #endregion

            #region Process RouteBag
            RouteBag _routeBag = null;
            if (!String.IsNullOrEmpty(routeBag))
            {
                _routeBag = JsonConvert.DeserializeObject<RouteBag>(routeBag);
                ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
            }
            #endregion

            var auth = await _context.Auths.FindAsync(id);
            _context.Auths.Remove(auth);
            await _context.SaveChangesAsync();

            #region Router v1.0
            if (_routeBag != null)
            {
                if (_routeBag.Routes.Count() > 0)
                {
                    RouteEl routeEl = new RouteEl();

                    switch (_routeBag.Routes.Count())
                    {
                        case 0:
                            return RedirectToAction(nameof(Index));
                        case 1:
                            routeEl = _routeBag.Routes[0]; //list index starts with 0
                            _routeBag.Routes.Remove(routeEl);
                            ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
                            break;
                        case 2:
                            routeEl = _routeBag.Routes[1]; //list index starts with 0
                            _routeBag.Routes.Remove(routeEl);
                            ViewBag.routeBag = JsonConvert.SerializeObject(_routeBag);
                            break;
                        default:
                            return RedirectToAction(nameof(Index));
                    }

                    if (routeEl.Id > 0)
                    {
                        return RedirectToAction(routeEl.Action.ToString(), _routeBag.TranslateRoute(routeEl.RoutePath), new { ID = routeEl.Id, ViewBag.routeBag });
                    }
                    else
                    {
                        return RedirectToAction(routeEl.Action.ToString(), _routeBag.TranslateRoute(routeEl.RoutePath), new { ViewBag.routeBag });
                    }
                }
            }
            #endregion

            return RedirectToAction(nameof(Index));
        }

        private bool AuthExists(int id)
        {
            return _context.Auths.Any(e => e.ReportAuthID == id);
        }

        private void AuthenticateUser()
        {
            var username = User.Identity.Name;

            ADconn ADuser = new ADconn(username);

            ViewBag.Name = ADuser.DisplayName;
            ViewBag.EmailAddress = ADuser.Email;
            ViewBag.Groups = ADuser.Groups;
            ViewBag.Department = ADuser.Department;
            ViewBag.AuthGroup = ADuser.AuthGroup;
        }
    }
}
