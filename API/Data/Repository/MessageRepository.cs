using API.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repository.Interface;
using DTOs;
using Entities;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(
            DataContext context,
            IMapper mapper
        )
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDTO>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username
                    && u.RecipientDelted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username
                    && u.SenderDelted == false),
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username
                    && u.RecipientDelted == false
                    && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(
                messages,
                messageParams.PageNumber,
                messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(
            string currentUsername,
            string recipientUsername
        )
        {
            var messages = await _context.Messages
                .Where(m => m.Recipient.UserName == currentUsername
                    && m.RecipientDelted == false
                    && m.Sender.UserName == recipientUsername
                    || m.Recipient.UserName == recipientUsername
                    && m.Sender.UserName == currentUsername
                    && m.SenderDelted == false)
                .Select(m => new MessageDTO{
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderUsername = m.SenderUsername,
                    SenderPhotoUrl = m.Sender.Photos.FirstOrDefault(x => x.IsMain).Url,
                    RecipientId = m.RecipientId,
                    RecipientUsername = m.RecipientUsername,
                    RecipientPhotoUrl = m.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url,
                    Content = m.Content,
                    DateRead = m.DateRead,
                    MessageSent = m.MessageSent
                })
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages
                .Where(m => m.DateRead == null
                    && m.RecipientUsername == currentUsername)
                .ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return messages;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}