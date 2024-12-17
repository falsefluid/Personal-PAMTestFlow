CREATE DATABASE dbPAMTestFlow;
USE dbPAMTestFlow;


CREATE TABLE TypeBlocks (
TypeID int not null identity primary key,
TypeName varchar(25) not null,
TypeDescription varchar(250),
NoRows int
);

CREATE TABLE Templates (
TemplateID int not null identity primary key,
TemplateName varchar(150) not null,
CreatedDate datetime,
UpdatedDate datetime null,
WebsiteURL varchar(200),
);

CREATE TABLE BuildingBlocks (
    BlockID INT IDENTITY(1,1) PRIMARY KEY,
	BlockName varchar(100) not null,
	TypeID int,
    TemplateID INT NOT NULL,
    SequenceOrder INT NOT NULL,
	BlockStatus int,
	WaitInterval int DEFAULT 0,
	FOREIGN KEY (TemplateID) REFERENCES Templates(TemplateID),
	FOREIGN KEY (TypeID) REFERENCES TypeBlocks (TypeID)
);

CREATE TABLE UserInputs (
	InputID int identity primary key,
	BlockID int,
	UserInput varchar(500),
	FOREIGN KEY (BlockID) REFERENCES BuildingBlocks(BlockID)
);


SELECT * FROM BuildingBlocks;

INSERT INTO TypeBlocks (TypeName, TypeDescription, NoRows)
VALUES
('Click','Simulates a mouse click on an element identified by a selector.', 1),
('Type Text','Types specified text into an input field identified by a selector.', 2),
('Press Key', 'Sends a keyboard key press (e.g., Enter, Tab) to a specified element.', 1),
('Hover','Moves the mouse cursor over an element without clicking.', 1),
('Navigate to URL','Opens a specific webpage URL in the browser for testing.', 1),
('Check Text Content','Retrieves and verifies the text content of an element matches the expected value.', 2),
('Wait for Selector','Waits until an element identified by the selector is visible or appears in view.', 1),
('Check Checkbox','Selects or checks a checkbox element.', 2),
('Select Dropdown Option','Chooses an option from a dropdown menu.', 2),
('File Upload','Simulates selecting a file to upload through an input element.', 2),
('Screenshot','Captures a screenshot of the current webpage and saves it as an image file.', 1),
('Generate PDF','Converts the current webpage to a PDF and saves it.', 1),
('Device Emulation','Configures the browser context to emulate a specific device, like an iPhone.', 1),
('Geolocation','Simulates a location by setting latitude and longitude coordinates.', 2);

SELECT * FROM Templates;

UPDATE Typeblocks
SET NoRows = 1
WHERE TypeName = 'Wait for Selector';

--DROP TABLE UserInputs;
--DROP TABLE BuildingBlocks;
--DROP TABLE Templates;
--DROP TABLE TypeBlocks;



-- Mock Data (if you have to drop tables)
