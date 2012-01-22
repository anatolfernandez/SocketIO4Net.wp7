// SocketIO4Net - nodejs SocketIO Server
// version: v0.5.17
// node.exe v0.6.7

// require command-line options to start this application
var argv = require('optimist')
    .usage('Usage: $0 -host [localhost] -port [3000]')
    .demand(['host', 'port'])
    .argv;

var express = require('express')
  , server = express.createServer()
  , socketio = require('socket.io')

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
io.set('log level', 1);


console.log('');
console.log('Nodejs Version: ',process.version);
console.log('     Listening: http://',argv.host, ':', argv.port);
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

    //io.sockets.emit('newConnection'); // broadcast to all clients
    socket.emit('news', { hello: 'world' });  // only the current connected socket will receive this event

    socket.on('event1', function (data) {
        console.log('On event1: ', data);
    });

    socket.on('event2', function (data) {
        console.log('On event2: ' , data);
    });

    socket.on('messageAck', function (data, fn) {
        console.log('messageAck: ' + data);
        if (fn != 'undefined') {
            console.log(fn);
            fn('woot');
        }
    });

    socket.on('disconnect', function () {
        io.sockets.emit('userdisconnected'); // broadcast event to all clients (no data)
    });
    
});





