using Lesson3_CNLTWeb.Data;
using Lesson3_CNLTWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lesson3_CNLTWeb.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomRepository _roomRepository;

        public RoomController(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public IActionResult Index(
            string? keyword,
            int? roomTypeId,
            bool? isAvailable,
            decimal? maxPrice,
            string? sortOrder)
        {
            PopulateRoomTypes(roomTypeId);
            ViewBag.Keyword = keyword;
            ViewBag.SelectedRoomTypeId = roomTypeId;
            ViewBag.SelectedIsAvailable = isAvailable;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortOrder = sortOrder;

            var rooms = _roomRepository.SearchRooms(keyword, roomTypeId, isAvailable, maxPrice, sortOrder);
            if (!rooms.Any())
            {
                ViewBag.EmptyMessage = "No rooms matched the search conditions.";
            }

            return View(rooms);
        }

        public IActionResult Detail(int id)
        {
            var room = _roomRepository.GetRoomById(id);
            if (room == null)
            {
                TempData["ErrorMessage"] = "RoomId does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        public IActionResult Create()
        {
            PopulateRoomTypes();
            return View(new Room_BIT242520 { IsAvailable = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room_BIT242520 room)
        {
            ValidateRoom(room);
            if (!ModelState.IsValid)
            {
                PopulateRoomTypes(room.RoomTypeId);
                return View(room);
            }

            _roomRepository.AddRoom(room);
            TempData["SuccessMessage"] = "Room created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var room = _roomRepository.GetRoomById(id);
            if (room == null)
            {
                TempData["ErrorMessage"] = "RoomId does not exist.";
                return RedirectToAction(nameof(Index));
            }

            PopulateRoomTypes(room.RoomTypeId);
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Room_BIT242520 room)
        {
            if (id != room.Id)
            {
                TempData["ErrorMessage"] = "Invalid room data.";
                return RedirectToAction(nameof(Index));
            }

            ValidateRoom(room, room.Id);
            if (!ModelState.IsValid)
            {
                PopulateRoomTypes(room.RoomTypeId);
                return View(room);
            }

            if (!_roomRepository.UpdateRoom(room))
            {
                TempData["ErrorMessage"] = "RoomId does not exist.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Room updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var room = _roomRepository.GetRoomById(id);
            if (room == null)
            {
                TempData["ErrorMessage"] = "RoomId does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!_roomRepository.DeleteRoom(id))
            {
                TempData["ErrorMessage"] = "RoomId does not exist.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Room deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddImage(int roomId, string imageUrl, bool isThumbnail)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                TempData["ErrorMessage"] = "Image URL is required.";
                return RedirectToAction(nameof(Detail), new { id = roomId });
            }

            if (!_roomRepository.AddRoomImage(roomId, imageUrl.Trim(), isThumbnail))
            {
                TempData["ErrorMessage"] = "RoomId does not exist.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Image added successfully.";
            return RedirectToAction(nameof(Detail), new { id = roomId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetThumbnail(int roomId, int imageId)
        {
            if (!_roomRepository.SetThumbnail(roomId, imageId))
            {
                TempData["ErrorMessage"] = "RoomImageId does not exist.";
                return RedirectToAction(nameof(Detail), new { id = roomId });
            }

            TempData["SuccessMessage"] = "Thumbnail updated successfully.";
            return RedirectToAction(nameof(Detail), new { id = roomId });
        }

        private void ValidateRoom(Room_BIT242520 room, int? exceptId = null)
        {
            if (!_roomRepository.RoomTypeExists(room.RoomTypeId))
            {
                ModelState.AddModelError(nameof(room.RoomTypeId), "RoomTypeId does not exist.");
            }

            if (!string.IsNullOrWhiteSpace(room.Name) &&
                _roomRepository.RoomNameExistsInType(room.Name, room.RoomTypeId, exceptId))
            {
                ModelState.AddModelError(nameof(room.Name), "Room name must be unique inside the selected room type.");
            }
        }

        private void PopulateRoomTypes(int? selectedId = null)
        {
            ViewBag.RoomTypes = new SelectList(_roomRepository.GetRoomTypes(), "Id", "Name", selectedId);
        }
    }
}
