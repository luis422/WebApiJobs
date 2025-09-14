namespace WebApiJobs.DTOs
{
    public class JobInfoResponseDTO<T>
    {
        public T? Detalhes { get; set; }

        public JobInfoResponseDTO()
        {
        }
    }
}
