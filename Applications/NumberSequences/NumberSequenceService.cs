using Indotalent.Data;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Models.Entities;

namespace Indotalent.Applications.NumberSequences
{
    public class NumberSequenceService : Repository<NumberSequence>
    {

        private readonly object lockObject = new object();

        public NumberSequenceService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditColumnTransformer auditColumnTransformer) :
                base(
                    context,
                    httpContextAccessor,
                    auditColumnTransformer)
        {
        }

        private NumberSequence? GetNumberSequence(string entityName, string prefix, string suffix)
        {
            return _context.NumberSequence
                .FirstOrDefault(ns => ns.EntityName == entityName && ns.Prefix == prefix && ns.Suffix == suffix);
        }

        private void UpdateNumberSequence(NumberSequence sequence)
        {
            sequence.LastUsedCount++;
            _context.SaveChanges();
        }

        private NumberSequence InsertNumberSequence(string entityName, string prefix, string suffix)
        {
            NumberSequence newSequence = new NumberSequence
            {
                EntityName = entityName,
                Prefix = prefix,
                Suffix = suffix,
                LastUsedCount = 1
            };

            _context.NumberSequence.Add(newSequence);
            _context.SaveChanges();

            return newSequence;
        }

        public string GenerateNumber(string entityName, string prefix, string suffix, bool useDate = true, int padding = 4)
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentException("Parameter entityName must not be null");
            }

            lock (lockObject)
            {
                NumberSequence? sequence = GetNumberSequence(entityName, prefix, suffix);

                if (sequence != null)
                {
                    UpdateNumberSequence(sequence);
                }
                else
                {
                    sequence = InsertNumberSequence(entityName, prefix, suffix);
                }

                string formattedNumber = $"{prefix}{sequence.LastUsedCount.ToString().PadLeft(padding, '0')}{(useDate ? DateTime.Now.ToString("yyyyMMdd") : "")}{suffix}";
                result = formattedNumber;
            }

            return result;
        }

    }
}
