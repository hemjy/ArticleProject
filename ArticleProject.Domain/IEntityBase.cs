﻿

namespace ArticleProject.Domain
{
     public class EntityBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? LastModified { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
