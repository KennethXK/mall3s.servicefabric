using Mall3s.Common.Enum;
using Mall3s.Common.Extension;
using Mall3s.Core.System.Manager;
using Mall3s.Core.System.Service;
using Mall3s.FriendlyException;
using Mall3s.LinqBuilder;
using Mall3s.Message.Entitys;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Mall3s.Core.Message.Service.Impl
{
    /// <summary>
    /// 系统消息
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public class MessageCoreService : IMessageCoreService
    {
        private readonly ISqlSugarRepository<MessageEntity> _messageRepository;
        private readonly ISqlSugarRepository<MessageReceiveEntity> _messageReceiveRepository;
        private readonly SqlSugarScope db;
        private readonly IUserCoreManager _userManager;
        private readonly IUsersCoreService _usersService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageRepository"></param>
        /// <param name="messageReceiveRepository"></param>
        /// <param name="usersService"></param>
        /// <param name="userManager"></param>
        public MessageCoreService(ISqlSugarRepository<MessageEntity> messageRepository, ISqlSugarRepository<MessageReceiveEntity> messageReceiveRepository, IUsersCoreService usersService, IUserCoreManager userManager)
        {
            _messageRepository = messageRepository;
            _messageReceiveRepository = messageReceiveRepository;
            db = messageRepository.Context;
            _usersService = usersService;
            _userManager = userManager;
        }

        #region PublicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<MessageEntity>> GetList(int type)
        {
            return await _messageRepository.Entities.Where(m => m.Type == type && m.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<MessageEntity> GetInfo(string id)
        {
            return await _messageRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 默认公告(app)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public string GetInfoDefaultNotice()
        {
            var entity = _messageRepository.Entities.Where(x => x.Type == 1 && x.DeleteMark == null).OrderBy(x => x.CreatorTime, OrderByType.Desc).First();
            return entity == null ? "" : entity.Title;
        }

        /// <summary>
        /// 默认消息(app)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public string GetInfoDefaultMessage(string userId)
        {
            var entity = db.Queryable<MessageEntity, MessageReceiveEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.Id == b.MessageId))
                .Where((a, b) => a.Type == 2 && a.DeleteMark == null && b.UserId == userId).OrderBy(a => a.CreatorTime, OrderByType.Desc).Select(a => a).First();
            return entity == null ? "" : entity.Title;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        [NonAction]
        public async Task<int> Delete(MessageEntity entity)
        {
            try
            {
                db.BeginTran();
                await _messageReceiveRepository.DeleteAsync(x => x.MessageId == entity.Id);
                var total = await _messageRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                db.CommitTran();
                return total;
            }
            catch (Exception)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        [NonAction]
        public async Task<int> Create(MessageEntity entity)
        {
            return await _messageRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="receiveEntityList">收件用户</param>
        [NonAction]
        public async Task<int> Create(MessageEntity entity, List<MessageReceiveEntity> receiveEntityList)
        {
            try
            {
                db.BeginTran();
                await _messageReceiveRepository.InsertAsync(receiveEntityList);
                var total = await _messageRepository.Context.Insertable(entity).CallEntityMethod(m => m.Create()).ExecuteCommandAsync();
                db.CommitTran();
                return total;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        [NonAction]
        public async Task<int> Update(MessageEntity entity)
        {
            return await _messageRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="receiveEntityList">收件用户</param>
        [NonAction]
        public async Task<int> Update(MessageEntity entity, List<MessageReceiveEntity> receiveEntityList)
        {
            try
            {
                db.BeginTran();
                await _messageReceiveRepository.InsertAsync(receiveEntityList);
                var total = await _messageRepository.Context.Updateable(entity).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                db.CommitTran();
                return total;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 消息已读（单条）
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="messageId">消息主键</param>
        [NonAction]
        public async Task MessageRead(string userId, string messageId)
        {
            await db.Updateable<MessageReceiveEntity>().SetColumns(it => it.ReadCount == it.ReadCount + 1).SetColumns(x => new MessageReceiveEntity()
            {
                IsRead = 1,
                ReadTime = DateTime.Now
            }).Where(x => x.UserId == userId && x.MessageId == messageId).ExecuteCommandAsync();

        }

        /// <summary>
        /// 消息已读（全部）
        /// </summary>
        /// <param name="id">id</param>
        [NonAction]
        public async Task MessageRead(string id)
        {
            try
            {
                db.BeginTran();
                var whereParams = LinqExpression.And<MessageReceiveEntity>();
                whereParams = whereParams.And(x => x.UserId == _userManager.UserId && x.IsRead == 0);
                if (id.IsNotEmptyOrNull())
                    whereParams = whereParams.And(x => x.MessageId == id);
                await db.Updateable<MessageReceiveEntity>().SetColumns(it => it.ReadCount == it.ReadCount + 1).SetColumns(x => new MessageReceiveEntity()
                {
                    IsRead = 1,
                    ReadTime = DateTime.Now
                }).Where(whereParams).ExecuteCommandAsync();

                db.CommitTran();
            }
            catch (Exception e)
            {
                db.RollbackTran();
            }
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="messageIds">消息Id</param>
        [NonAction]
        public async Task<int> DeleteRecord(string userId, List<string> messageIds)
        {
            return await _messageReceiveRepository.DeleteAsync(m => m.UserId == userId && messageIds.Contains(m.MessageId));
        }

        /// <summary>
        /// 获取未读数量（含 通知公告、系统消息）
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> GetUnreadCount(string userId)
        {
            return await _messageReceiveRepository.CountAsync(m => m.UserId == userId && m.IsRead == 0);
        }

        /// <summary>
        /// 获取公告未读数量
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public int GetUnreadNoticeCount(string userId)
        {
            return db.Queryable<MessageEntity, MessageReceiveEntity>((m, mr) => new JoinQueryInfos(JoinType.Left, m.Id == mr.MessageId)).Select((m, mr) => new { mr.Id, mr.UserId, mr.IsRead, m.Type, m.DeleteMark }).MergeTable().Where(x => x.Type == 1 && x.DeleteMark == null && x.UserId == userId && x.IsRead == 0).Count();
        }

        /// <summary>
        /// 获取消息未读数量
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public int GetUnreadMessageCount(string userId)
        {
            return db.Queryable<MessageEntity, MessageReceiveEntity>((m, mr) => new JoinQueryInfos(JoinType.Left, m.Id == mr.MessageId)).Select((m, mr) => new { mr.Id, mr.UserId, mr.IsRead, m.Type, m.DeleteMark }).MergeTable().Where(x => x.Type == 2 && x.DeleteMark == null && x.UserId == userId && x.IsRead == 0).Count();
        }

        /// <summary>
        /// 发送公告
        /// </summary>
        /// <param name="toUserIds">发送用户</param>
        /// <param name="entity">消息信息</param>
        /*[NonAction]
        public async Task SentNotice(List<string> toUserIds, MessageEntity entity)
        {
            try
            {
                entity.EnabledMark = 1;
                List<MessageReceiveEntity> receiveEntityList = new List<MessageReceiveEntity>();
                foreach (var item in toUserIds)
                {
                    MessageReceiveEntity messageReceiveEntity = new MessageReceiveEntity();
                    messageReceiveEntity.Id = YitIdHelper.NextId().ToString();
                    messageReceiveEntity.MessageId = entity.Id;
                    messageReceiveEntity.UserId = item;
                    messageReceiveEntity.IsRead = 0;
                    receiveEntityList.Add(messageReceiveEntity);
                }
                await Update(entity, receiveEntityList);
                //消息推送 - PC端
                foreach (var item in WebSocketClientCollection._clients.FindAll(it => it.TenantId == _userManager.TenantId))
                {
                    if (toUserIds.Contains(item.UserId))
                    {
                        await item.SendMessageAsync(new { method = "messagePush", userId = _userManager.UserId, toUserId = toUserIds, title = entity.Title, unreadNoticeCount = 1 }.Serialize());
                    }
                }
            }
            catch (Exception ex)
            {
                throw Mall3sException.Oh(ErrorCode.D7003);
            }
        }*/

        /// <summary>
        /// 发送站内消息
        /// </summary>
        /// <param name="toUserIds">发送用户</param>
        /// <param name="title">标题</param>
        /// <param name="bodyText">内容</param>
        /*[NonAction]
        public async Task SentMessage(List<string> toUserIds, string title, string bodyText = null)
        {
            try
            {
                MessageEntity entity = new MessageEntity();
                entity.Id = YitIdHelper.NextId().ToString();
                entity.Title = title;
                entity.BodyText = bodyText;
                entity.Type = 2;
                entity.LastModifyTime = DateTime.Now;
                entity.LastModifyUserId = _userManager.UserId;
                List<MessageReceiveEntity> receiveEntityList = new List<MessageReceiveEntity>();
                foreach (var item in toUserIds)
                {
                    MessageReceiveEntity messageReceiveEntity = new MessageReceiveEntity();
                    messageReceiveEntity.Id = YitIdHelper.NextId().ToString();
                    messageReceiveEntity.MessageId = entity.Id;
                    messageReceiveEntity.UserId = item;
                    messageReceiveEntity.IsRead = 0;
                    receiveEntityList.Add(messageReceiveEntity);
                }
                await Create(entity, receiveEntityList);
                //消息推送 - PC端
                foreach (var item in WebSocketClientCollection._clients)
                {
                    if (toUserIds.Contains(item.UserId))
                    {
                        await item.SendMessageAsync(new { method = "messagePush", userId = _userManager.UserId, toUserId = toUserIds, title = entity.Title, unreadNoticeCount = 1 }.Serialize());
                    }
                }
                //消息推送 - APP
                // GetuiAppPushHelper.Instance.SendNotice(userInfo.TenantId, toUserIds, "系统消息", entity.F_Title, "2");
            }
            catch (Exception ex)
            {
                throw;
            }
        }*/

        #endregion
    }
}
