
using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using FirstApp.Application.MailSettings;
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
        IMailJetService _mailJetService,
        IEmailTemplateRendererAsyync _emailTemplateRendererAsyync,
        INotificationsRepository _notificationsRepository,
        ICounterReportsRepository counterReportsRepository
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

                var model = new
                {
                    Email = "rahilriyaz27@gmail.com",
                    Content = "Your video has been temporarily deleted beacuse it have been reported by many if u think it is unjustified you can counter report in settings > reporting > select a report then file a counter report!"
                };

                var settings = new MailSetting()
                {
                    Body = await _emailTemplateRendererAsyync.EmailTemplate("Report.cshtml", model),
                    Subject = "Booking",
                    To = new List<string>() { "rahiltechnochords@gmail.com" },
                };


                await _mailJetService.SendEmailAsync(settings);

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
            var reply = await _commentRepliesRepository.FindOneAsync(commentId);

            if (reply is null)
                return;

            var comment = await _commentsRepository.FindOneAsync(reply.CommentId);

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
                reply.IsDeleted = true;
                await _commentRepliesRepository.UpdateAsync(reply);

                var notification = new Notification
                {
                    Title = "A comment reply has been removed temporarily from your video because it has been reported. You can counter report for this.",
                    UserId = channel.Owner,
                    VideoId = video.Id,
                    CommentId = commentId,
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
                    ReportType = ReportType.Reply,
                };

                await _ownerReportsRepository.InsertAsync(newReport);
            }
            else
            {
                report.ReportCount++;
                await _ownerReportsRepository.UpdateAsync(report);
            }
        }

        public async Task<APIResponse<int>> CounterReport(CounterReportRequest model)
        {
            //var userId = _contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("Unauthorized");

            var hasAlreadyCountered = await counterReportsRepository.ExistsAsync
                (_ => _.EntityId == new ObjectId(model.EntityId) && _.CounteredBy == userId);

            if (hasAlreadyCountered)
                return APIResponse<int>.ErrorResponse("You have already countered this report wait for the response.Thank you");

            var counterReport = new CounterReport
            {
                CounteredBy = userId,
                EntityId = new ObjectId(model.EntityId),
                ReportType = model.ReportType,
                Justification = model.Justification,
            };

            var addedCounteredReportResponse = await counterReportsRepository.InsertAsync(counterReport);

            return addedCounteredReportResponse > 0 ? APIResponse<int>.
                SuccessResponse(addedCounteredReportResponse,
                "You have successfully countered against this report.Our team will look upon this if they find this unjustified your content will be recovered and you will be notified.Thank you!"):
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<List<OwnerReportResponse>>> GetOwnerReportsAsync()
        {
            //var userId = _contextService.GetUserId();
            var userId = ObjectId.Parse("6639a524cb32b4eca722a251");

            if (userId == ObjectId.Empty)
                return APIResponse<List<OwnerReportResponse>>.ErrorResponse("Unauthorized");

            var reports = await _ownerReportsRepository.GetOwnerReportsAsync(userId);

            var response = reports.Select(_ => new OwnerReportResponse
            {
                CommentTitle = _.CommentTitle,
                CreatedAt = _.CreatedAt,
                EntityId = _.EntityId.ToString(),
                Id = _.Id.ToString(),
                IsPermanentlyRemoved = _.IsPermanentlyRemoved,
                IsRestored = _.IsRestored,
                IsTemporarilyRemoved = _.IsTemporarilyRemoved,
                OwnerId = _.OwnerId.ToString(),
                PermanentlyRemovedAt = _.PermanentlyRemovedAt,
                ReplyTitle = _.ReplyTitle,
                ReportCount = _.ReportCount,
                ReportType = _.ReportType,
                RestoredAt = _.RestoredAt,
                TemporarilyRemovedAt = _.TemporarilyRemovedAt,
                UpdatedAt = _.UpdatedAt,
                VideoTitle = _.VideoTitle
            }).ToList();

            return APIResponse<List<OwnerReportResponse>>.SuccessResponse(response, "Owner reports fetched");
        }
    }
}

