﻿using Dapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class InteractionRepository : IInteractionRepository
    {
        private readonly NpgsqlConnection _connection;

        public InteractionRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(Interaction interaction)
        {
            await _connection.OpenAsync();

            var sql = @"
                INSERT INTO interactions (user1_id, user2_id, status)
                VALUES (@User1Id, @User2Id, @Status)
                RETURNING id;
            ";

            var interactionId = await _connection.QuerySingleAsync<int>(sql, new
            {
                interaction.User1Id,
                interaction.User2Id,
                Status = interaction.Status.ToString()
            });

            return interactionId;
        }

        public async Task<bool> Delete(int id)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM interactions WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }

        public async Task<Interaction> GetById(int id)
        {
            await _connection.OpenAsync();

            var sql = @"
                SELECT id, user1_id, user2_id, status
                FROM interactions
                WHERE id = @Id;
            ";

            var interaction = await _connection.QuerySingleOrDefaultAsync<Interaction>(sql, new { Id = id });
            return interaction;
        }

        public async Task<IEnumerable<Interaction>> GetByStatus(Status status)
        {
            await _connection.OpenAsync();

            var sql = @"
                SELECT id, user1_id, user2_id, status
                FROM interactions
                WHERE status = @Status;
            ";

            var interactions = await _connection.QueryAsync<Interaction>(sql, new { Status = status.ToString() });
            return interactions;
        }

        public async Task<bool> Update(Interaction interaction)
        {
            await _connection.OpenAsync();

            var sql = @"
                UPDATE interactions
                SET user1_id = @User1Id,
                    user2_id = @User2Id,
                    status = @Status
                WHERE id = @Id;
            ";

            var affectedRows = await _connection.ExecuteAsync(sql, new
            {
                interaction.User1Id,
                interaction.User2Id,
                Status = interaction.Status.ToString(), 
                interaction.Id
            });

            return affectedRows > 0;
        }

        public async Task<IEnumerable<Interaction>> GetAll()
        {
            await _connection.OpenAsync();

            var sql = @"
                SELECT id, user1_id, user2_id, status
                FROM interactions;
            ";

            var interactions = await _connection.QueryAsync<Interaction>(sql);
            return interactions;
        }
    }
}