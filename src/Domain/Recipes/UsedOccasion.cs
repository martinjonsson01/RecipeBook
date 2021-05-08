using System;

namespace RecipeBook.Core.Domain.Recipes
{
    public class UsedOccasion : BaseEntity, IShallowCloneable<UsedOccasion>, IEquatable<UsedOccasion>
    {
        public DateTime Date     { get; set; } = DateTime.MinValue;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public string?  Comment  { get; set; }

        public UsedOccasion ShallowClone()
        {
            return new()
            {
                Id = Id,
                Date = Date,
                Duration = Duration,
                Comment = Comment
            };
        }

        public bool Equals(UsedOccasion? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Date.Equals(other.Date) &&
                   Duration.Equals(other.Duration) &&
                   (
                       Comment is null && other.Comment is null ||
                       Comment is not null && Comment.Equals(other.Comment)
                   );
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UsedOccasion);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ (Comment != null ? Comment.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}