﻿using CodeBE_COMP1640.Controllers.PermissionController;
using CodeBE_COMP1640.Models;
using System;
using System.Collections.Generic;

namespace CodeBE_COMP1640.Controllers.CommentController;

public partial class CommentDTO
{
    public int CommentId { get; set; }

    public int? ArticleId { get; set; }

    public int? UserId { get; set; }

    public string? CommentContent { get; set; }

    public DateTime? CommentTime { get; set; }

    public UserForCommentDTO? UserForComment { get; set; }

    public CommentDTO() { }

    public CommentDTO(Comment Comment)
    {
        CommentId = Comment.CommentId;
        ArticleId = Comment.ArticleId;
        UserId = Comment.UserId;
        CommentContent = Comment.CommentContent;
        CommentTime = Comment.CommentTime;
        UserForComment = Comment.User == null ? null : new UserForCommentDTO(Comment.User);
    }
}

public class UserForCommentDTO
{
    public string? UserName { get; set; }
    public UserForCommentDTO() { }
    public UserForCommentDTO(User user) {
        this.UserName = user.Username;
    }
}
