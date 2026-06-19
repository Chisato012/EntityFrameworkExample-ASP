using Lesson3_CNLTWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace Lesson3_CNLTWeb.Data
{
    public class RoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<RoomType_BIT242520> GetRoomTypes()
        {
            return _context.RoomTypes
                .OrderBy(t => t.Name)
                .ToList();
        }

        public RoomType_BIT242520? GetRoomTypeById(int id)
        {
            return _context.RoomTypes
                .Include(t => t.Rooms)
                .FirstOrDefault(t => t.Id == id);
        }

        public bool RoomTypeExists(int id)
        {
            return _context.RoomTypes.Any(t => t.Id == id);
        }

        public bool RoomTypeNameExists(string name, int? exceptId = null)
        {
            return _context.RoomTypes.Any(t => t.Name == name && (!exceptId.HasValue || t.Id != exceptId.Value));
        }

        public void AddRoomType(RoomType_BIT242520 roomType)
        {
            _context.RoomTypes.Add(roomType);
            _context.SaveChanges();
        }

        public bool UpdateRoomType(RoomType_BIT242520 roomType)
        {
            var existing = _context.RoomTypes.Find(roomType.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Name = roomType.Name;
            existing.Description = roomType.Description;
            _context.SaveChanges();
            return true;
        }

        public bool DeleteRoomType(int id)
        {
            var roomType = _context.RoomTypes
                .Include(t => t.Rooms)
                .FirstOrDefault(t => t.Id == id);

            if (roomType == null || roomType.Rooms.Any())
            {
                return false;
            }

            _context.RoomTypes.Remove(roomType);
            _context.SaveChanges();
            return true;
        }

        public List<Room_BIT242520> SearchRooms(
            string? keyword,
            int? roomTypeId,
            bool? isAvailable,
            decimal? maxPrice,
            string? sortOrder)
        {
            var query = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomImages)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(r => r.Name.Contains(keyword));
            }

            if (roomTypeId.HasValue)
            {
                query = query.Where(r => r.RoomTypeId == roomTypeId.Value);
            }

            if (isAvailable.HasValue)
            {
                query = query.Where(r => r.IsAvailable == isAvailable.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(r => r.Price <= maxPrice.Value);
            }

            query = sortOrder switch
            {
                "price_asc" => query.OrderBy(r => r.Price).ThenBy(r => r.Id),
                "price_desc" => query.OrderByDescending(r => r.Price).ThenBy(r => r.Id),
                "area_desc" => query.OrderByDescending(r => r.Area).ThenBy(r => r.Id),
                _ => query.OrderBy(r => r.Id)
            };

            return query.ToList();
        }

        public Room_BIT242520? GetRoomById(int id)
        {
            return _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomImages.OrderByDescending(i => i.IsThumbnail).ThenBy(i => i.Id))
                .FirstOrDefault(r => r.Id == id);
        }

        public bool RoomNameExistsInType(string name, int roomTypeId, int? exceptId = null)
        {
            return _context.Rooms.Any(r =>
                r.Name == name &&
                r.RoomTypeId == roomTypeId &&
                (!exceptId.HasValue || r.Id != exceptId.Value));
        }

        public void AddRoom(Room_BIT242520 room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
        }

        public bool UpdateRoom(Room_BIT242520 room)
        {
            var existing = _context.Rooms.Find(room.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Name = room.Name;
            existing.Price = room.Price;
            existing.Area = room.Area;
            existing.IsAvailable = room.IsAvailable;
            existing.Description = room.Description;
            existing.RoomTypeId = room.RoomTypeId;
            _context.SaveChanges();
            return true;
        }

        public bool DeleteRoom(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return false;
            }

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return true;
        }

        public bool AddRoomImage(int roomId, string imageUrl, bool isThumbnail)
        {
            if (!_context.Rooms.Any(r => r.Id == roomId))
            {
                return false;
            }

            using var transaction = _context.Database.BeginTransaction();
            if (isThumbnail)
            {
                ClearThumbnails(roomId);
                _context.SaveChanges();
            }

            _context.RoomImages.Add(new RoomImage_BIT242520
            {
                RoomId = roomId,
                ImageUrl = imageUrl,
                IsThumbnail = isThumbnail
            });
            _context.SaveChanges();
            transaction.Commit();
            return true;
        }

        public bool SetThumbnail(int roomId, int imageId)
        {
            var image = _context.RoomImages.FirstOrDefault(i => i.Id == imageId && i.RoomId == roomId);
            if (image == null)
            {
                return false;
            }

            using var transaction = _context.Database.BeginTransaction();
            ClearThumbnails(roomId);
            _context.SaveChanges();

            image.IsThumbnail = true;
            _context.SaveChanges();
            transaction.Commit();
            return true;
        }

        private void ClearThumbnails(int roomId)
        {
            foreach (var image in _context.RoomImages.Where(i => i.RoomId == roomId && i.IsThumbnail))
            {
                image.IsThumbnail = false;
            }
        }
    }
}
