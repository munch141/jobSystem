namespace WebApp
{
    public class Job {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public Job(Guid id, string name, DateTime createdAt)
        {
            Id = id;
            Name = name;
            CreatedAt = createdAt;
        }

        public Task GetTask() {
            var task = this.Name switch
            {
                _ => Task.Delay(TimeSpan.FromSeconds(30))
            };

            return task;
        }
    }
}