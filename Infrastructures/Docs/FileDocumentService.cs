using Indotalent.Data;

namespace Indotalent.Infrastructures.Docs
{
    public class FileDocumentService : IFileDocumentService
    {
        private readonly ApplicationDbContext _context;

        public FileDocumentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> UploadDocumentAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            if (!IsValidDocumentFile(file))
            {
                throw new ArgumentException("Invalid file type. Only Word, Excel, and PDF files are allowed.");
            }

            if (file.Length > 10 * 1024 * 1024) // 10 MB in bytes
            {
                throw new ArgumentException("File size exceeds the maximum allowed size of 10 MB.");
            }

            var document = new FileDocument
            {
                Id = Guid.NewGuid(),
                OriginalFileName = file.FileName
            };

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                document.DocumentData = memoryStream.ToArray();
            }

            _context.FileDocument.Add(document);
            await _context.SaveChangesAsync();

            return document.Id;
        }

        public async Task<Guid> UpdateDocumentAsync(Guid? documentId, IFormFile? newFile)
        {
            if (documentId == null)
            {
                throw new ArgumentException("Invalid documentId");
            }

            if (newFile == null || newFile.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            if (!IsValidDocumentFile(newFile))
            {
                throw new ArgumentException("Invalid file type. Only Word, Excel, and PDF files are allowed.");
            }

            if (newFile.Length > 10 * 1024 * 1024) // 10 MB in bytes
            {
                throw new ArgumentException("File size exceeds the maximum allowed size of 10 MB.");
            }

            var existingDocument = await _context.FileDocument.FindAsync(documentId);

            if (existingDocument == null)
            {
                throw new ArgumentException($"Document with ID {documentId} not found");
            }

            // Update the existing document with the new data
            existingDocument.OriginalFileName = newFile.FileName;

            using (var memoryStream = new MemoryStream())
            {
                await newFile.CopyToAsync(memoryStream);
                existingDocument.DocumentData = memoryStream.ToArray();
            }

            await _context.SaveChangesAsync();

            return existingDocument.Id;
        }

        public async Task<FileDocument> GetDocumentAsync(Guid? id)
        {
            if (id == null)
            {
                id = new Guid(Guid.Empty.ToString());
            }

            var fileDocument = await _context.FileDocument.FindAsync(id);

            if (fileDocument == null)
            {
                var defaultDocumentPath = Path.Combine("wwwroot", "nodocument.txt");

                fileDocument = new FileDocument
                {
                    Id = Guid.Empty,
                    OriginalFileName = "NoDocument.txt",
                    DocumentData = File.ReadAllBytes(defaultDocumentPath)
                };
            }

            return fileDocument;
        }

        private bool IsValidDocumentFile(IFormFile file)
        {
            var allowedDocumentTypes = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
            return allowedDocumentTypes.Contains(file.ContentType);
        }
    }
}
