﻿using System;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class NotificationItemVM
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
