using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Domain.Common.Base
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
        
    }
}
