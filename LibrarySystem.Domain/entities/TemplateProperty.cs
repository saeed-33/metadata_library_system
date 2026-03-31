namespace LibrarySystem.Domain.Entities
{
    public class TemplateProperty
    {
        public int TemplateId { get; set; }
        public ResourceTemplate Template { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }

        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }
}