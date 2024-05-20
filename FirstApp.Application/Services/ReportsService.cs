
using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using FirstApp.Domain.Enums;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FirstApp.Application.Services
{
    public class ReportsService
        (
        IContextService _contextService,
        IReportsRepository _reportsRepository,
        IVideousRepository _videousRepository,
        ICommentsRepository _commentsRepository,
        IChannelsRepository _channelsRepository,
        ICommentRepliesRepository _commentRepliesRepository,
        IOwnerReportsRepository _ownerReportsRepository,
        INotificationsRepository _notificationsRepository
        )
        : IReportsService
    {
        #region Old Code
        /* public async Task<APIResponse<int>> AddReport(ReportRequest model)
         {
             //var userId = contextService.GetUserId();
             var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

             if (userId == ObjectId.Empty)
                 return APIResponse<int>.ErrorResponse("Unauthorized");

            *//* var hasAlreadyReported = await reportsRepository.ExistsAsync
                 (_ => _.EntityId == new ObjectId(model.EntityId));

             if (hasAlreadyReported)
                 return APIResponse<int>.ErrorResponse($"You have already reported");*//*

             var newReport = new Report
             {
                 EntityId = new ObjectId(model.EntityId),
                 ReportContentType = new ObjectId(model.ReportContentType),
                 ReportedBy = userId,
                 ReportedOn = model.ReportedOn
             };

             var insertedReportResponse = await reportsRepository.InsertAsync(newReport);

             if (insertedReportResponse <= 0)
                 return APIResponse<int>.ErrorResponse();

             var noOfReports = await reportsRepository.CheckNoOfReportsForTheEntity(model.EntityId);

             if (noOfReports == 2)
             {
                 switch (model.ReportedOn)
                 {
                     case ReportType.Video:
                         var video = await videousRepository.FindOneAsync(new ObjectId(model.EntityId));

                         if (video is null)
                             return APIResponse<int>.ErrorResponse("Invalid videoid");

                         video.IsDeleted = true;

                         var updatedVideoResponse = await videousRepository.UpdateAsync(video);

                         if(updatedVideoResponse > 0)
                         {
                             var channel = await channelsRepository.FindOneAsync(video.ChannelId);

                             var notification = new Notification
                             {
                                 Title = "Your video has been removed temporarily because you have been reported if you think you are unjustified please file a counter report.Thank you !",
                                 UserId = channel!.Owner,
                                 VideoId = video.Id,
                             };

                             await notificationsRepository.InsertAsync(notification);

                         }


                     break;

                     case ReportType.Comment:
                         var comment = await commentsRepository.FindOneAsync(new ObjectId(model.EntityId));

                         if(comment is null)
                             return APIResponse<int>.ErrorResponse("Invalid commentid");

                         comment.IsDeleted = true;

                         var updatedCommentResponse = await commentsRepository.UpdateAsync(comment);

                         if(updatedCommentResponse > 0)
                         {
                          var video_1 = await videousRepository.FindOneAsync(comment.VideoId);

                             if (video_1 is null)
                                 return APIResponse<int>.ErrorResponse("Couldn't find owner");

                             var channel = await channelsRepository.FindOneAsync(video_1.ChannelId);

                             var notification = new Notification
                             {
                                 Title = "A comment has been removed temporarily from your video because it has been reported.You can counter report for this.",
                                 UserId = channel!.Owner,
                                 VideoId = video_1.Id,
                                 CommentId = comment.Id,
                             };

                             await notificationsRepository.InsertAsync(notification);

                         }
                         break;

                     case ReportType.Reply:
                     break;
                 }
             }

             return APIResponse<int>.SuccessResponse
             (insertedReportResponse, "Thanks. We've received your report. If we find this content to be in violation of our Community Guidelines, we will remove it.");
         }*/

        #endregion Old Code



        public async Task<APIResponse<int>> AddReport(ReportRequest model)
        {
            //var userId = _contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("Unauthorized");

            var newReport = new Report
            {
                EntityId = new ObjectId(model.EntityId),
                ReportContentType = new ObjectId(model.ReportContentType),
                ReportedBy = userId,
                ReportedOn = model.ReportedOn
            };

            var insertedReportResponse = await _reportsRepository.InsertAsync(newReport);

            if (insertedReportResponse <= 0)
                return APIResponse<int>.ErrorResponse();

            var noOfReports = await _reportsRepository.CheckNoOfReportsForTheEntity(model.EntityId);

            switch (model.ReportedOn)
            {
                case ReportType.Video:
                    await HandleVideoReport(new ObjectId(model.EntityId), noOfReports);
                    break;

                case ReportType.Comment:
                    await HandleCommentReport(new ObjectId(model.EntityId), noOfReports);
                    break;

                case ReportType.Reply:
                    await HandleReplyReport(new ObjectId(model.EntityId), noOfReports);
                    break;

                default:
                    break;
            }

            return APIResponse<int>.SuccessResponse(insertedReportResponse,
                "Thanks. We've received your report. If we find this content to be in violation of our Community Guidelines, we will remove it.");
        }


        private async Task HandleVideoReport(ObjectId videoId, int noOfReports)
        {
            var video = await _videousRepository.FindOneAsync(videoId);

            if (video is null)
                return;

            var channel = await _channelsRepository.FindOneAsync(video.ChannelId);

            if (channel is null)
                return;

            var report = await _ownerReportsRepository.FirstOrDefaultAsync
                (_ => _.EntityId == videoId && _.OwnerId == channel.Owner);


            if (noOfReports == 2)
            {

                video.IsDeleted = true;
                await _videousRepository.UpdateAsync(video);

                var notification = new Notification
                {
                    Title = "Your video has been removed temporarily because you have been reported if you think you are unjustified please file a counter report. Thank you!",
                    UserId = channel.Owner,
                    VideoId = video.Id,
                };

                await _notificationsRepository.InsertAsync(notification);

                report!.IsTemporarilyRemoved = true;
                report.TemporarilyRemovedAt = DateTime.Now;
            }


            if (report is null)
            {
                var newReport = new OwnerReports
                {
                    OwnerId = channel.Owner,
                    EntityId = videoId,
                    ReportCount = 1,
                    ReportType = ReportType.Video,
                };

                await _ownerReportsRepository.InsertAsync(newReport);
            }
            else
            {
                report.ReportCount++;
                await _ownerReportsRepository.UpdateAsync(report);
            }

        }

        private async Task HandleCommentReport(ObjectId commentId, int noOfReports)
        {
            var comment = await _commentsRepository.FindOneAsync(commentId);

            if (comment is null)
                return;

            var video = await _videousRepository.FindOneAsync(comment.VideoId);

            if (video is null)
                return;

            var channel = await _channelsRepository.FindOneAsync(video.ChannelId);

            if (channel is null)
                return;

            var report = await _ownerReportsRepository.FirstOrDefaultAsync
               (_ => _.EntityId == commentId && _.OwnerId == channel.Owner);


            if (noOfReports == 2)
            {
                comment.IsDeleted = true;
                await _commentsRepository.UpdateAsync(comment);

                var notification = new Notification
                {
                    Title = "A comment has been removed temporarily from your video because it has been reported. You can counter report for this.",
                    UserId = channel.Owner,
                    VideoId = video.Id,
                    CommentId = comment.Id,
                };

                await _notificationsRepository.InsertAsync(notification);

                report!.IsTemporarilyRemoved = true;
                report.TemporarilyRemovedAt = DateTime.Now;
            }

            if (report is null)
            {
                var newReport = new OwnerReports
                {
                    OwnerId = channel.Owner,
                    EntityId = commentId,
                    ReportCount = 1,
                    ReportType = ReportType.Video,
                };

                await _ownerReportsRepository.InsertAsync(newReport);
            }
            else
            {
                report.ReportCount++;
                await _ownerReportsRepository.UpdateAsync(report);

            }

        }

        private async Task HandleReplyReport(ObjectId commentId, int noOfReports)
        {

        }

    }
}

