using System;
using ToileDeFond.Modularity;

namespace ToileDeFond.ContentManagement
{
    public class Publication : IComparable<Publication>
    {
        #region Data

        public DateTime EndingDate { get; protected internal set; }

        public DateTime StartingDate { get; protected set; }

        //Permet d'ordonner les publication de manière à ce qu'il puisse y avoir des publication conflictuelles et qu'on puisse choisir la plus récente
        public DateTime CreationDate { get; protected set; }

        #endregion

        #region ctors

        public Publication(DateTime? startingDate = null, DateTime? endingDate = null, DateTime? creationDate = null)
        {
            var now = DependencyResolver.Current.GetService<IDateTimeManager>().Now();

            startingDate = startingDate ?? now;
            endingDate = endingDate ?? DateTime.MaxValue;

            if (endingDate <= startingDate)
            {
                throw new ArgumentException("A publication must end after it begins...", "endingDate");
            }

            StartingDate = startingDate.Value;
            EndingDate = endingDate.Value;
            CreationDate = creationDate ?? now;
        }

        protected Publication()
        {
            
        }

        #endregion

        #region Public

        public int CompareTo(Publication other)
        {
            return CreationDate.CompareTo(other.CreationDate);
        }

        public static bool operator <(Publication left, Publication right)
        {
            return left.CreationDate < right.CreationDate;
        }

        public static bool operator >(Publication left, Publication right)
        {
            return left.CreationDate > right.CreationDate;
        }

        public static bool operator <=(Publication left, Publication right)
        {
            return left.CreationDate <= right.CreationDate;
        }

        public static bool operator >=(Publication left, Publication right)
        {
            return left.CreationDate >= right.CreationDate;
        }

        #endregion
    }
}