Public Enum LoginNotices As Integer
    login_ok = 0
    blocked = 3 'ID deleted or blocked
    wrong_pw = 4 'Incorrect password
    not_registered = 5 'Not a registered id
    system_error_1 = 6 'System error
    already_logged = 7 'Already logged in
    system_error_2 = 8 'System error
    system_error_3 = 9 'System error
    server_limit = 10 ' Cannot process so many connections
    only_older = 11 'Only users older than 20 can use this channel
    unable_to_log_gm = 13 'Unable to log on as master at this ip
    wrong_gateway_1 = 14 'Wrong gateway or personal info
    processing_request = 15 'Processing request
    email_verify_1 = 16 'Please verify your account through email...
    wrong_gateway_2 = 17 'Wrong gateway or personal info
    email_verify_2 = 21 'Please verify your account through email...
    license = 23 'License agreement
    eublock_notice = 25 'Maple Europe notice
    trial = 27 'Some weird full client notice, probably for trial versions
End Enum
