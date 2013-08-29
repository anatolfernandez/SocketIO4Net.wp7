var app = require('express')()
  , server = require('http').createServer(app)
  , io = require('socket.io').listen(server);

server.listen(1337, '10.30.200.81');

app.get('/', function (req, res) {
    res.send('the cake is a lie');
});

io.sockets.on('connection', function (socket) {
    socket.on('echo', function (data) {
        console.log('echo: ' + data);
        socket.emit('echo', 'Hello from server: ' + data);
    });
});