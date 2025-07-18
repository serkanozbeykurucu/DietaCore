﻿namespace DietaCore.Dto.MealDTOs
{
    public class MealRequestDto
    {
        public string Title { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public int Calories { get; set; }
        public decimal Proteins { get; set; }
        public decimal Carbohydrates { get; set; }
        public decimal Fats { get; set; }
        public int DietPlanId { get; set; }
    }
}
