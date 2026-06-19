using Lesson3_CNLTWeb.Data;
using Lesson3_CNLTWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lesson3_CNLTWeb.Controllers
{
    public class RoomTypeController : Controller
    {
        private readonly RoomRepository _roomRepository;

        public RoomTypeController(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public IActionResult Index()
        {
            return View(_roomRepository.GetRoomTypes());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RoomType_BIT242520 roomType)
        {
            ValidateRoomType(roomType);
            if (!ModelState.IsValid)
            {
                return View(roomType);
            }

            _roomRepository.AddRoomType(roomType);
            TempData["SuccessMessage"] = "Room type created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var roomType = _roomRepository.GetRoomTypeById(id);
            if (roomType == null)
            {
                TempData["ErrorMessage"] = "RoomTypeId does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(roomType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RoomType_BIT242520 roomType)
        {
            if (id != roomType.Id)
            {
                TempData["ErrorMessage"] = "Invalid room type data.";
                return RedirectToAction(nameof(Index));
            }

            ValidateRoomType(roomType, roomType.Id);
            if (!ModelState.IsValid)
            {
                return View(roomType);
            }

            if (!_roomRepository.UpdateRoomType(roomType))
            {
                TempData["ErrorMessage"] = "RoomTypeId does not exist.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Room type updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var roomType = _roomRepository.GetRoomTypeById(id);
            if (roomType == null)
            {
                TempData["ErrorMessage"] = "RoomTypeId does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(roomType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!_roomRepository.DeleteRoomType(id))
            {
                TempData["ErrorMessage"] = "Cannot delete a room type that is being used by rooms.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Room type deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void ValidateRoomType(RoomType_BIT242520 roomType, int? exceptId = null)
        {
            if (!string.IsNullOrWhiteSpace(roomType.Name) &&
                _roomRepository.RoomTypeNameExists(roomType.Name, exceptId))
            {
                ModelState.AddModelError(nameof(roomType.Name), "Room type name already exists.");
            }
        }
    }
}
