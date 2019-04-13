using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

/// <summary>
/// 消息中心
/// </summary>
public class HubHelper : Hub
{
    /// <summary>
    /// 游戏逻辑
    /// </summary>
    static GameLogic gameLogic;
    static bool isStart = false;


    /// <summary>
    /// 获取当前用户Id
    /// </summary>
    /// <param name="data"></param>
    public void GetId(Data data)
    {
        Clients.Caller.SendAsync("GetId", Context.ConnectionId);
    }
    /// <summary>
    /// 获取当前连接所有用户
    /// </summary>
    /// <param name="data"></param>
    public void GetALLUser(Data data)
    {
        if (gameLogic != null)
            Clients.Caller.SendAsync("GetALLUser", gameLogic.list);
    }
    /// <summary>
    /// 用户状态改变
    /// </summary>
    /// <param name="user">用户信息</param>
    public void UpdateSate(UserInfo user)
    {
        gameLogic.UpdateUserInfo(Context.ConnectionId, user);
    }
    /// <summary>
    /// 用户加入
    /// </summary>
    /// <param name="data"></param>
    public void Join(Data data)
    {
        if (!isStart)
        {
            isStart = true;
            gameLogic = new GameLogic(this.Clients.All);
        }
        gameLogic.JoinUser(this.Context.ConnectionId, data);

    }

    /// <summary>
    /// 用户断开
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override Task OnDisconnectedAsync(Exception exception)
    {
        if (gameLogic != null)
            gameLogic.LevelUser(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
/// <summary>
/// 游戏逻辑
/// </summary>
public class GameLogic
{
    /// <summary>
    /// 全局客户端通信信息
    /// </summary>
    readonly IClientProxy gloable;
    /// <summary>
    /// 游戏定时器
    /// </summary>
    readonly Timer gameTimer;
    /// <summary>
    /// 所有用户
    /// </summary>
    public List<UserInfo> list = new List<UserInfo>();

    public GameLogic(IClientProxy _gloable)
    {
        gloable = _gloable;
        //启动模拟游戏帧
        gameTimer = new Timer(30);
        gameTimer.Elapsed += GameTimer_Elapsed;
        gameTimer.Start();
    }

    private void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            //发送已改变状态用户信息广播
            if (item.Change)
            {
                item.Change = false; 
                gloable.SendAsync("update", list[i]);
            }
        }
    }
    /// <summary>
    /// 用户加入
    /// </summary>
    /// <param name="id">用户编号</param>
    /// <param name="data">加入信息</param>
    public void JoinUser(string id, Data data)
    {
        //随机获取角色图片
        var user = new UserInfo() { Id = id, Name = data.Name, Role = "role" + new Random().Next(1, 4) + ".jpg", Change = true };
        list.Add(user);
        gloable.SendAsync("add", user);
    }
    /// <summary>
    /// 用户断开处理
    /// </summary>
    /// <param name="id">用户编号</param>
    public void LevelUser(string id)
    {
        var user = list.FirstOrDefault(mm => mm.Id == id);
        if (user != null)
        {
            gloable.SendAsync("level", user);
            list.Remove(user);
        }
    }
    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="id">用户编号</param>
    /// <param name="p">用户信息</param>
    public void UpdateUserInfo(string id, UserInfo changeUser)
    {
        var user = list.FirstOrDefault(mm => mm.Id == id);
        if (user != null)
        {
            user.X = changeUser.X;
            user.Y = changeUser.Y;
            user.Change = true;
        }
    }
}
/// <summary>
///用户信息
/// </summary>
public class UserInfo
{
    public string Id { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 用户角色图片
    /// </summary>
    public string Role { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    /// <summary>
    /// 是否动作变化
    /// </summary>
    public bool Change { get; set; }
}
/// <summary>
/// 传输数据
/// </summary>
public class Data
{
    public string Name { get; set; }
    public string Value { get; set; }
}
     