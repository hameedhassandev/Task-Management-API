using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Test
{
    [TestFixture]
    public class TaskRepositoryTests
    {
        private AppDbContext _context;
        private TaskRepository _repository;
        private Guid _userId;
        private Guid _taskId1;
        private Guid _taskId2;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new TaskRepository(_context);

            _userId = Guid.NewGuid();
            _taskId1 = Guid.NewGuid();
            _taskId2 = Guid.NewGuid();

            SeedData();
        }

        private void SeedData()
        {
            _context.Users.Add(new User
            {
                Id = _userId,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = Guid.NewGuid().ToString(),
            });

            var tasks = new List<TaskEntity>
            {
                new TaskEntity
                {
                    Id = _taskId1,
                    Title = "Independent Task1",
                    CreatedByUserId = _userId,
                    OrganizationId = null,
                    Status = Task_Status.Pending,
                    ProjectId = null
                },
                new TaskEntity
                {
                    Id = _taskId2,
                    Title = "Independent Task2",
                    CreatedByUserId = _userId,
                    OrganizationId = null,
                    Status = Task_Status.Completed,
                    ProjectId = null
                },
            };
            _context.Tasks.AddRange(tasks);

            _context.SaveChanges();
        }


        [Test]
        public async Task GetIndependentUserTasks_ReturnsAllTasks_WhenUserExists()
        {
            // Act
            var result = await _repository.GetTasksForIndividualUser(_userId);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.That(result.Value?.Count, Is.EqualTo(2));
            Assert.That(result.Value?.Any(t => t.Task.Title == "Independent Task1"), Is.True);
            Assert.That(result.Value?.Any(t => t.Task.Title == "Independent Task2"), Is.True);
        }

        [Test]
        public async Task GetIndependentUserTasks_ReturnsCompletedTask_WhenUserExistsAndHasCompletedTask()
        {
    
            // Act
            var result = await _repository.GetTasksForIndividualUser(_userId);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.That(result.Value?.Any(t => t.Task.Title == "Independent Task2" && t.Task.Status == Task_Status.Completed), Is.True);
        }

        [Test]
        public async Task GetIndependentUserTasks_ReturnsFailure_WhenUserDoesNotExist()
        {
            // Act
            var result = await _repository.GetTasksForIndividualUser(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.That(result.Message, Is.EqualTo("User not found"));
        }
    }
}
