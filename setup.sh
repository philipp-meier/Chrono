# Ensures all required files are created.
mkdir -p ./data
mkdir -p ./https

if [ ! -f ./data/chrono.db ]; then
    sqlite3 data/chrono.db "VACUUM;"
    echo "Created chrono.db."
else
    echo "chrono.db already exists."
fi

if [ ! -f ./https/aspnetapp.pfx ]; then
    dotnet dev-certs https -ep ./https/aspnetapp.pfx -p defaultPassword
else
    echo "Valid https certificate already exists."
fi