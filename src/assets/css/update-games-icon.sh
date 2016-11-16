echo 'TRUNCATE icon;'
echo 'INSERT INTO icon VALUES '
cat game-icons.css  | grep :before | sed -r 's/^.game-icon-//g' |sed -r 's/:before \{//'  | sort |  sed -r "s/(.+)/('\1'),/"
echo ';'

