create table Tags (
	FK_Name varchar(255) NOT NULL,
    Tag varchar(255) NOT NULL,
    foreign key (FK_Name) references recipe_db.recipes(Name)
    )