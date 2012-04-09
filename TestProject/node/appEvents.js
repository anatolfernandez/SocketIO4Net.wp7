// SocketIO4Net - nodejs SocketIO Server
// version: v0.5.20
// node.exe v0.6.10

// require command-line options to start this application
var argv = require('optimist')
    .usage('Usage: $0 -host [localhost] -port [3000]')
    .demand(['host', 'port'])
    .argv;

var express = require('express')
  , server = express.createServer()
  , socketio = require('socket.io');

  // configure Express
server.configure(function () {
    server.use(express.bodyParser());
    server.use('/content', express.static(__dirname + '/content'));
    server.use('/scripts', express.static(__dirname + '/scripts'));
    });

// start server listening at host:port
server.listen(argv.port, argv.host); // http listen on host:port e.g. http://localhost:3000/

// configure Socket.IO
var io = socketio.listen(server); // start socket.io
io.set('log level', 4);


console.log('');
console.log('Nodejs Version: ',process.version);
console.log('     Listening: http://',argv.host, ':', argv.port);
console.log('    Socket.IO : v',socketio.version);
console.log('');

// ***************************************************************
//    WEB Handlers  
//    Express guid: http://expressjs.com/guide.html
// ***************************************************************
server.get('/', function (req, res) {
    res.sendfile(__dirname + '/appEventsClient.html');
});



// ***************************************************************
//    Socket.IO Client Handlers
// ***************************************************************
io.sockets.on('connection', function (socket) {

    socket.on('partInfo', function (data) {
        console.log('recv [socket].[partInfo]  data = {0} \r\n'.format(JSON.stringify(data)));
        data.Level = 3;
        io.sockets.emit('update', data);  // broadcast event to all clients
    });

    socket.on('simple', function (data) {
        console.log('recv [socket].[simple]  data = {0} \r\n'.format(data));
    });

    socket.on('messageAck', function (data, fn) {
        console.log('[root].[messageAck]: {0}'.format(JSON.stringify(data)));

        if (fn != 'undefined') {
            console.log('  sending ack message \r\n');
            fn('hello son, {0}'.format(data.hello)); // return payload
        }
        else {
            console.log(' ** expecting return function to call, but was missing?');
        }
    });

    socket.on('disconnect', function () {
        console.log('client disconnected [root] namespace');
        io.sockets.emit('clientdisconnected'); // broadcast event to all clients (no data)
    });

});

// ***************************************************************
//    Socket.IO Namesspace 'logger'
// ***************************************************************
var logger = io
  .of('/logger')
  .on('connection', function (nsSocket) {
      console.log('client connected to [logger] namespace');
      logger.emit('traceEvent', new eventLog({
          eventCode: 'connection',
          messageText: 'logger namespace'
      }));  // all 'logger' clients will receive this event

      nsSocket.on('disconnect', function () {
          console.log('client disconnected from [logger] namespace');
          logger.emit('traceEvent', new eventLog({
              eventCode: 'disconnect',
              messageText: 'logger namespace'
          }));  // all 'logger' clients will receive this event
      });

      nsSocket.on('messageAck', function (data, fn) {
          console.log('[logger].[messageAck]: {0}'.format(JSON.stringify(data)));
          
          if (fn != 'undefined') {
              console.log('  sending ack message \r\n');
              fn('hello son, {0}'.format( data.hello));
          } else {
              console.log(' ** expecting return function to call, but was missing?');
          }

          // also bcast traceEvent message
          logger.emit('traceEvent', new eventLog({
              eventCode: 'messageAck',
              messageText: 'logger namespace'
          }));  // all 'logger' clients will receive this event
      });
      
  });

  // simple object to pass on events - matches our C# object
  function eventLog(obj) {
      this.eventCode; // map to socket.io onEvents: onSignOn, sessionStart, instructorJoin, pollingStart
      this.msgText; // 
      this.timeStamp = new Date().toLocaleTimeString();

      // IF AN OBJECT was passed then initialize properties from that object
      for (var prop in obj) {
          this[prop] = obj[prop];
      }
  };

  // simple Part object - matches our C# object
  function Part(obj) {
      this.PartNumber;
      this.Code;
      this.Level;
      // IF AN OBJECT was passed then initialize properties from that object
      for (var prop in obj) {
          this[prop] = obj[prop];
      }
  };

  //************************************
  //        Helpers 
  // ***********************************
  // Inspired by http://bit.ly/juSAWl
  // Augment String.prototype to allow for easier formatting.  This implementation 
  // doesn't completely destroy any existing String.prototype.format functions,
  // and will stringify objects/arrays.
  String.prototype.format = function () {
      var args = arguments;
      return this.replace(/{(\d+)}/g, function (match, number) {
          return typeof args[number] != 'undefined'
      ? args[number]
      : match
    ;
      });
  };


