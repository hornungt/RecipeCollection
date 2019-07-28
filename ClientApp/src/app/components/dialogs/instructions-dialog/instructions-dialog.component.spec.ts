/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { InstructionsDialogComponent } from './instructions-dialog.component';

let component: InstructionsDialogComponent;
let fixture: ComponentFixture<InstructionsDialogComponent>;

describe('instructions-dialog component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ InstructionsDialogComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(InstructionsDialogComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});