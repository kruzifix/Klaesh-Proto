const idBuffer = {}
module.exports = function(type) {
    if (!(type in idBuffer))
        idBuffer[type] = 0
    return idBuffer[type]++
}